// Họ và tên: [Điền tên Thành viên 1]
// Mã sinh viên: [Điền MSSV]
// Nội dung thực hiện: Tài khoản, Đăng nhập, Đăng xuất, Phân quyền

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Filters;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.ViewModels;

namespace QuanLyNhaHang.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoan/DangNhap
        [HttpGet]
        public IActionResult DangNhap()
        {
            // Nếu đã đăng nhập rồi thì không cần vào lại trang login
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        // POST: /TaiKhoan/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm tài khoản theo tên đăng nhập trước (dùng LINQ + EF Core)
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            if (taiKhoan.MatKhau != model.MatKhau)
            {
                ModelState.AddModelError(string.Empty, "Sai mật khẩu.");
                return View(model);
            }

            // Đăng nhập thành công -> lưu tối thiểu vào Session
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro.ToString());

            if (taiKhoan.VaiTro == VaiTro.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /TaiKhoan/DangXuat
        [HttpGet]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ thông tin đăng nhập khỏi Session
            return RedirectToAction("DangNhap");
        }

        // GET: /TaiKhoan/TruyCapBiTuChoi
        [HttpGet]
        public IActionResult TruyCapBiTuChoi()
        {
            return View();
        }

        // ================== QUẢN LÝ TÀI KHOẢN (CHỈ ADMIN) ==================

        // GET: /TaiKhoan
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> Index(string? tuKhoa)
        {
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(t => t.TenDangNhap.Contains(tuKhoa) || t.HoTen.Contains(tuKhoa));
            }

            var danhSach = await query.OrderBy(t => t.TenDangNhap).ToListAsync();
            ViewBag.TuKhoa = tuKhoa;
            return View(danhSach);
        }

        // GET: /TaiKhoan/Details/5
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> Details(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // GET: /TaiKhoan/Create
        [SessionAuthorize(VaiTro.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> Create(TaiKhoan model)
        {
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.TaiKhoans.Add(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Tạo tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TaiKhoan/Edit/5
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // POST: /TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> Edit(int id, TaiKhoan model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            bool trungTen = await _context.TaiKhoans
                .AnyAsync(t => t.TenDangNhap == model.TenDangNhap && t.MaTaiKhoan != id);
            if (trungTen)
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.TaiKhoans.Update(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Cập nhật tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /TaiKhoan/DoiTrangThai/5  (Khóa / Mở khóa tài khoản)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(VaiTro.Admin)]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            taiKhoan.TrangThai = !taiKhoan.TrangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
