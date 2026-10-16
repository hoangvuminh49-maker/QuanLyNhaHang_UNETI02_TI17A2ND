// Họ và tên: Đoàn Văn Hiệp, Nguyễn Huy Hoàng 
// Mã sinh viên: 23274800003 ,22203100058
// Nội dung thực hiện: Đặt bàn của khách hàng, lịch sử đặt bàn (Module 3) và Quản lý phiếu đặt bàn - Xác nhận, Hủy, Cập nhật trạng thái sử dụng bàn (Module 4)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    public class PhieuDatBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuDatBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Kiểm tra trùng lịch: một bàn không được có hai phiếu còn hiệu lực (khác Đã hủy)
        /// có khoảng thời gian giao nhau trong cùng một ngày sử dụng.
        /// Điều kiện giao nhau: GioBatDauMoi < GioKetThucCu VÀ GioKetThucMoi > GioBatDauCu.
        /// </summary>
        private async Task<bool> TrungLichAsync(int maBan, DateTime ngaySuDung, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? boQuaMaPhieu = null)
        {
            var query = _context.PhieuDatBans.Where(p =>
                p.MaBan == maBan &&
                p.NgaySuDung.Date == ngaySuDung.Date &&
                p.TrangThai != TrangThaiPhieu.DaHuy);

            if (boQuaMaPhieu.HasValue)
                query = query.Where(p => p.MaPhieu != boQuaMaPhieu.Value);

            return await query.AnyAsync(p => gioBatDau < p.GioKetThuc && gioKetThuc > p.GioBatDau);
        }

        // ================= KHU VỰC KHÁCH HÀNG =================

        [KhachHangOnly]
        [HttpGet]
        public async Task<IActionResult> DatBan(int? maBan)
        {
            ViewBag.DanhSachBan = new SelectList(
                await _context.BanAns.Where(b => b.TrangThai != TrangThaiBan.TamNgung).OrderBy(b => b.TenBan).ToListAsync(),
                "MaBan", "TenBan", maBan);

            return View(new DangKyDatBanViewModel { MaBan = maBan ?? 0, NgaySuDung = DateTime.Today });
        }

        [KhachHangOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatBan(DangKyDatBanViewModel model)
        {
            var maKh = HttpContext.Session.GetInt32(SessionKeys.MaKhachHang);
            if (maKh == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var khachHang = await _context.KhachHangs.FindAsync(maKh.Value);
            var ban = await _context.BanAns.FindAsync(model.MaBan);

            if (khachHang == null || !khachHang.TrangThai)
                ModelState.AddModelError(string.Empty, "Tài khoản khách hàng không hợp lệ hoặc đã bị khóa.");

            if (ban == null)
                ModelState.AddModelError(nameof(model.MaBan), "Bàn không tồn tại.");
            else if (ban.TrangThai == TrangThaiBan.TamNgung)
                ModelState.AddModelError(nameof(model.MaBan), "Bàn đang tạm ngừng phục vụ.");

            if (model.NgaySuDung.Date < DateTime.Today)
                ModelState.AddModelError(nameof(model.NgaySuDung), "Ngày sử dụng không được trước ngày hiện tại.");

            if (model.GioKetThuc <= model.GioBatDau)
                ModelState.AddModelError(nameof(model.GioKetThuc), "Giờ kết thúc phải sau giờ bắt đầu.");

            if (model.SoNguoi <= 0)
                ModelState.AddModelError(nameof(model.SoNguoi), "Số người phải lớn hơn 0.");

            if (ban != null && model.SoNguoi > ban.SoChoNgoi)
                ModelState.AddModelError(nameof(model.SoNguoi), $"Số người vượt quá sức chứa của bàn ({ban.SoChoNgoi} chỗ).");

            if (ban != null && ModelState.IsValid &&
                await TrungLichAsync(model.MaBan, model.NgaySuDung, model.GioBatDau, model.GioKetThuc))
            {
                ModelState.AddModelError(string.Empty, "Bàn đã được đặt trong khoảng thời gian này. Vui lòng chọn khung giờ hoặc bàn khác.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachBan = new SelectList(
                    await _context.BanAns.Where(b => b.TrangThai != TrangThaiBan.TamNgung).OrderBy(b => b.TenBan).ToListAsync(),
                    "MaBan", "TenBan", model.MaBan);
                return View(model);
            }

            var phieu = new PhieuDatBan
            {
                MaKhachHang = maKh.Value,
                MaBan = model.MaBan,
                NgayDat = DateTime.Now, // Hệ thống tự xác định, không cho khách nhập
                NgaySuDung = model.NgaySuDung.Date,
                GioBatDau = model.GioBatDau,
                GioKetThuc = model.GioKetThuc,
                SoNguoi = model.SoNguoi,
                GhiChu = model.GhiChu,
                TrangThai = TrangThaiPhieu.ChoXacNhan
            };

            _context.PhieuDatBans.Add(phieu);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đặt bàn thành công. Phiếu của bạn đang chờ xác nhận.";
            return RedirectToAction(nameof(LichSu));
        }

        [KhachHangOnly]
        public async Task<IActionResult> LichSu(string? trangThai)
        {
            var maKh = HttpContext.Session.GetInt32(SessionKeys.MaKhachHang);
            if (maKh == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var query = _context.PhieuDatBans
                .Include(p => p.BanAn)!.ThenInclude(b => b!.LoaiBan)
                .Where(p => p.MaKhachHang == maKh.Value)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

            ViewBag.TrangThai = trangThai;
            return View(await query.OrderByDescending(p => p.NgayDat).ToListAsync());
        }

        [DaDangNhap]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var maKh = HttpContext.Session.GetInt32(SessionKeys.MaKhachHang);
            var vaiTro = HttpContext.Session.GetString(SessionKeys.VaiTro);

            var phieu = await _context.PhieuDatBans
                .Include(p => p.BanAn)!.ThenInclude(b => b!.LoaiBan)
                .Include(p => p.KhachHang)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            // Khách hàng chỉ được xem phiếu đặt bàn thuộc tài khoản của chính mình
            if (vaiTro == VaiTro.KhachHang && phieu.MaKhachHang != maKh)
                return StatusCode(403);

            return View(phieu);
        }

        // ================= KHU VỰC ADMIN =================

        [AdminOnly]
        public async Task<IActionResult> Index(string? tenKhachHang, string? tenBan, string? trangThai, DateTime? ngaySuDung, int trang = 1)
        {
            var query = _context.PhieuDatBans
                .Include(p => p.KhachHang)
                .Include(p => p.BanAn)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tenKhachHang))
                query = query.Where(p => p.KhachHang != null && p.KhachHang.HoTen.Contains(tenKhachHang));

            if (!string.IsNullOrWhiteSpace(tenBan))
                query = query.Where(p => p.BanAn != null && p.BanAn.TenBan.Contains(tenBan));

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

            if (ngaySuDung.HasValue)
                query = query.Where(p => p.NgaySuDung.Date == ngaySuDung.Value.Date);

            query = query.OrderByDescending(p => p.NgayDat);

            var danhSach = await PagedList<PhieuDatBan>.CreateAsync(query, trang, 10);

            ViewBag.TenKhachHang = tenKhachHang;
            ViewBag.TenBan = tenBan;
            ViewBag.TrangThai = trangThai;
            ViewBag.NgaySuDung = ngaySuDung?.ToString("yyyy-MM-dd");

            return View(danhSach);
        }

        [AdminOnly]
        public async Task<IActionResult> Details(int id)
        {
            var phieu = await _context.PhieuDatBans
                .Include(p => p.BanAn)!.ThenInclude(b => b!.LoaiBan)
                .Include(p => p.KhachHang)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();
            return View(phieu);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id)
        {
            var phieu = await _context.PhieuDatBans
                .Include(p => p.BanAn).Include(p => p.KhachHang)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);
            if (phieu == null) return NotFound();

            if (phieu.TrangThai != TrangThaiPhieu.ChoXacNhan)
            {
                TempData["Loi"] = "Chỉ có thể xác nhận phiếu đang ở trạng thái Chờ xác nhận.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (phieu.KhachHang == null || !phieu.KhachHang.TrangThai)
            {
                TempData["Loi"] = "Khách hàng không còn hoạt động.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (phieu.BanAn == null || phieu.BanAn.TrangThai == TrangThaiBan.TamNgung)
            {
                TempData["Loi"] = "Bàn không còn phục vụ.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (phieu.SoNguoi > phieu.BanAn.SoChoNgoi)
            {
                TempData["Loi"] = "Số người vượt quá sức chứa của bàn.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (phieu.NgaySuDung.Date < DateTime.Today)
            {
                TempData["Loi"] = "Ngày sử dụng không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (await TrungLichAsync(phieu.MaBan, phieu.NgaySuDung, phieu.GioBatDau, phieu.GioKetThuc, phieu.MaPhieu))
            {
                TempData["Loi"] = "Không thể xác nhận vì trùng lịch với một phiếu khác.";
                return RedirectToAction(nameof(Details), new { id });
            }

            phieu.TrangThai = TrangThaiPhieu.DaXacNhan;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Xác nhận đặt bàn thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var phieu = await _context.PhieuDatBans.FindAsync(id);
            if (phieu == null) return NotFound();

            if (phieu.TrangThai == TrangThaiPhieu.DaSuDung || phieu.TrangThai == TrangThaiPhieu.DaHuy)
            {
                TempData["Loi"] = "Không thể hủy phiếu ở trạng thái hiện tại.";
                return RedirectToAction(nameof(Details), new { id });
            }

            phieu.TrangThai = TrangThaiPhieu.DaHuy;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Hủy phiếu đặt bàn thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangSuDungBan(int id)
        {
            var phieu = await _context.PhieuDatBans.Include(p => p.BanAn).FirstOrDefaultAsync(p => p.MaPhieu == id);
            if (phieu == null) return NotFound();

            if (phieu.TrangThai != TrangThaiPhieu.DaXacNhan)
            {
                TempData["Loi"] = "Chỉ có thể cập nhật bàn đang sử dụng cho phiếu đã xác nhận.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (phieu.BanAn != null) phieu.BanAn.TrangThai = TrangThaiBan.DangSuDung;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Cập nhật bàn sang trạng thái Đang sử dụng.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatDaSuDung(int id)
        {
            var phieu = await _context.PhieuDatBans.Include(p => p.BanAn).FirstOrDefaultAsync(p => p.MaPhieu == id);
            if (phieu == null) return NotFound();

            if (phieu.TrangThai != TrangThaiPhieu.DaXacNhan)
            {
                TempData["Loi"] = "Chỉ có thể cập nhật Đã sử dụng cho phiếu đã xác nhận.";
                return RedirectToAction(nameof(Details), new { id });
            }

            phieu.TrangThai = TrangThaiPhieu.DaSuDung;

            if (phieu.BanAn != null)
            {
                // Đưa bàn về Sẵn sàng nếu không còn phiếu nào khác đang chiếm bàn hôm nay
                bool conPhieuKhacDangChiemBan = await _context.PhieuDatBans.AnyAsync(p =>
                    p.MaBan == phieu.MaBan && p.MaPhieu != phieu.MaPhieu &&
                    p.NgaySuDung.Date == DateTime.Today && p.TrangThai == TrangThaiPhieu.DaXacNhan);

                if (!conPhieuKhacDangChiemBan)
                    phieu.BanAn.TrangThai = TrangThaiBan.SanSang;
            }

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Cập nhật trạng thái Đã sử dụng thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
