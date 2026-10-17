// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Quản lý bàn ăn, tìm kiếm, lọc, sắp xếp và phân trang (Module 2)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    public class BanAnController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int SoBanMoiTrang = 8;

        public BanAnController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Quy trình dùng chung: Truy vấn dữ liệu -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị.
        /// Toàn bộ được thực hiện trên IQueryable (LINQ to Entities), Skip()/Take() chạy dưới SQL Server,
        /// không tải toàn bộ dữ liệu về trình duyệt.
        /// </summary>
        private async Task<BanAnListViewModel> TimKiemLocSapXepPhanTrangAsync(
            string? tuKhoa, int? maLoaiBan, int? soChoNgoi, string? trangThai, string? sapXep,
            int trang, bool chiBanConPhucVu)
        {
            IQueryable<BanAn> query = _context.BanAns.Include(b => b.LoaiBan);

            if (chiBanConPhucVu)
                query = query.Where(b => b.TrangThai != TrangThaiBan.TamNgung);

            // ----- Tìm kiếm theo Tên bàn hoặc Vị trí -----
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tk = tuKhoa.Trim();
                query = query.Where(b => b.TenBan.Contains(tk) || (b.ViTri != null && b.ViTri.Contains(tk)));
            }

            // ----- Lọc: các điều kiện có thể kết hợp cùng lúc -----
            if (maLoaiBan.HasValue)
                query = query.Where(b => b.MaLoaiBan == maLoaiBan.Value);

            if (soChoNgoi.HasValue)
                query = query.Where(b => b.SoChoNgoi >= soChoNgoi.Value);

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(b => b.TrangThai == trangThai);

            // ----- Sắp xếp -----
            query = sapXep switch
            {
                "ten_za" => query.OrderByDescending(b => b.TenBan),
                "cho_tang" => query.OrderBy(b => b.SoChoNgoi).ThenBy(b => b.TenBan),
                "cho_giam" => query.OrderByDescending(b => b.SoChoNgoi).ThenBy(b => b.TenBan),
                _ => query.OrderBy(b => b.TenBan) // mặc định: Tên bàn A -> Z
            };

            // ----- Phân trang trên truy vấn EF Core (Skip/Take) -----
            var danhSachBan = await PagedList<BanAn>.CreateAsync(query, trang, SoBanMoiTrang);

            return new BanAnListViewModel
            {
                DanhSachBan = danhSachBan,
                TuKhoa = tuKhoa,
                MaLoaiBan = maLoaiBan,
                SoChoNgoi = soChoNgoi,
                TrangThai = trangThai,
                SapXep = sapXep,
                DanhSachLoaiBan = await _context.LoaiBans.OrderBy(l => l.TenLoaiBan).ToListAsync()
            };
        }

        // ================= KHU VỰC ADMIN =================

        [AdminOnly]
        public async Task<IActionResult> Index(string? tuKhoa, int? maLoaiBan, int? soChoNgoi, string? trangThai, string? sapXep, int trang = 1)
        {
            var vm = await TimKiemLocSapXepPhanTrangAsync(tuKhoa, maLoaiBan, soChoNgoi, trangThai, sapXep, trang, chiBanConPhucVu: false);
            return View(vm);
        }

        [AdminOnly]
        public async Task<IActionResult> Create()
        {
            ViewBag.DanhSachLoaiBan = new SelectList(await _context.LoaiBans.ToListAsync(), "MaLoaiBan", "TenLoaiBan");
            return View();
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BanAn model)
        {
            if (await _context.BanAns.AnyAsync(b => b.TenBan == model.TenBan))
                ModelState.AddModelError(nameof(model.TenBan), "Tên bàn đã tồn tại.");

            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachLoaiBan = new SelectList(await _context.LoaiBans.ToListAsync(), "MaLoaiBan", "TenLoaiBan", model.MaLoaiBan);
                return View(model);
            }

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Thêm bàn ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null) return NotFound();

            ViewBag.DanhSachLoaiBan = new SelectList(await _context.LoaiBans.ToListAsync(), "MaLoaiBan", "TenLoaiBan", banAn.MaLoaiBan);
            return View(banAn);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BanAn model)
        {
            if (id != model.MaBan) return NotFound();

            if (await _context.BanAns.AnyAsync(b => b.TenBan == model.TenBan && b.MaBan != id))
                ModelState.AddModelError(nameof(model.TenBan), "Tên bàn đã tồn tại.");

            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachLoaiBan = new SelectList(await _context.LoaiBans.ToListAsync(), "MaLoaiBan", "TenLoaiBan", model.MaLoaiBan);
                return View(model);
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.BanAns.AnyAsync(b => b.MaBan == id)) return NotFound();
                throw;
            }

            TempData["ThongBao"] = "Cập nhật bàn ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        public async Task<IActionResult> Delete(int id)
        {
            var banAn = await _context.BanAns.Include(b => b.LoaiBan).FirstOrDefaultAsync(b => b.MaBan == id);
            if (banAn == null) return NotFound();
            return View(banAn);
        }

        [AdminOnly]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null) return NotFound();

            bool coPhieuDatBan = await _context.PhieuDatBans.AnyAsync(p => p.MaBan == id);
            if (coPhieuDatBan)
            {
                TempData["Loi"] = "Không thể xóa bàn ăn vì đã có phiếu đặt bàn liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.BanAns.Remove(banAn);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Xóa bàn ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null) return NotFound();

            banAn.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= KHU VỰC KHÁCH HÀNG (tra cứu bàn) =================

        public async Task<IActionResult> DanhSach(string? tuKhoa, int? maLoaiBan, int? soChoNgoi, string? trangThai, string? sapXep, int trang = 1)
        {
            var vm = await TimKiemLocSapXepPhanTrangAsync(tuKhoa, maLoaiBan, soChoNgoi, trangThai, sapXep, trang, chiBanConPhucVu: true);
            return View(vm);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var banAn = await _context.BanAns.Include(b => b.LoaiBan).FirstOrDefaultAsync(b => b.MaBan == id);
            if (banAn == null) return NotFound();
            return View(banAn);
        }
    }
}
