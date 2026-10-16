// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Controller Tài khoản - Đăng nhập - Đăng xuất - Đăng ký - Thông tin cá nhân (Module 1)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan) != null)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            // Sử dụng Entity Framework Core + LINQ để kiểm tra tài khoản
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                return View(model);
            }
            if (taiKhoan.MatKhau != model.MatKhau)
            {
                ModelState.AddModelError(string.Empty, "Sai mật khẩu.");
                return View(model);
            }
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa.");
                return View(model);
            }

            // Lưu tối thiểu Mã tài khoản, Họ tên, Vai trò vào Session
            HttpContext.Session.SetInt32(SessionKeys.MaTaiKhoan, taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
            HttpContext.Session.SetString(SessionKeys.VaiTro, taiKhoan.VaiTro);

            if (taiKhoan.VaiTro == VaiTro.KhachHang)
            {
                var kh = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaTaiKhoan == taiKhoan.MaTaiKhoan);
                if (kh != null) HttpContext.Session.SetInt32(SessionKeys.MaKhachHang, kh.MaKhachHang);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DangXuat()
        {
            // Xóa toàn bộ thông tin đăng nhập khỏi Session
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan) != null)
                return RedirectToAction("Index", "Home");

            return View(new DangKyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = model.MatKhau,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = VaiTro.KhachHang,
                TrangThai = true
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var khachHang = new KhachHang
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        [DaDangNhap]
        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var maKh = HttpContext.Session.GetInt32(SessionKeys.MaKhachHang);
            if (maKh == null) return RedirectToAction("Index", "Home");

            var kh = await _context.KhachHangs.FindAsync(maKh.Value);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [DaDangNhap]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinCaNhan(KhachHang model)
        {
            var maKh = HttpContext.Session.GetInt32(SessionKeys.MaKhachHang);
            // Khách hàng chỉ được cập nhật thông tin của chính mình
            if (maKh == null || maKh.Value != model.MaKhachHang)
                return StatusCode(403);

            ModelState.Remove(nameof(KhachHang.MaTaiKhoan));
            if (!ModelState.IsValid) return View(model);

            var kh = await _context.KhachHangs.FindAsync(maKh.Value);
            if (kh == null) return NotFound();

            kh.HoTen = model.HoTen;
            kh.NgaySinh = model.NgaySinh;
            kh.GioiTinh = model.GioiTinh;
            kh.SoDienThoai = model.SoDienThoai;
            kh.Email = model.Email;
            kh.DiaChi = model.DiaChi;

            await _context.SaveChangesAsync();

            // Đồng bộ họ tên hiển thị trong Session
            HttpContext.Session.SetString(SessionKeys.HoTen, kh.HoTen);

            TempData["ThongBao"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction("ThongTinCaNhan");
        }
    }
}
