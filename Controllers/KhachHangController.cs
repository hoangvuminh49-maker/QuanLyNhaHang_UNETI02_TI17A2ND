// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: Controller Quản lý khách hàng - xem danh sách, chi tiết, tìm kiếm, cập nhật trạng thái (Module 3)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    [AdminOnly]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin: xem danh sách + tìm kiếm bằng LINQ
        public async Task<IActionResult> Index(string? tuKhoa)
        {
            var query = _context.KhachHangs.Include(k => k.TaiKhoan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tk = tuKhoa.Trim();
                query = query.Where(k => k.HoTen.Contains(tk)
                    || (k.SoDienThoai != null && k.SoDienThoai.Contains(tk))
                    || (k.Email != null && k.Email.Contains(tk)));
            }

            ViewBag.TuKhoa = tuKhoa;
            return View(await query.OrderBy(k => k.HoTen).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.PhieuDatBans)!.ThenInclude(p => p.BanAn)
                .FirstOrDefaultAsync(k => k.MaKhachHang == id);

            if (khachHang == null) return NotFound();
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            var khachHang = await _context.KhachHangs.Include(k => k.TaiKhoan)
                .FirstOrDefaultAsync(k => k.MaKhachHang == id);
            if (khachHang == null) return NotFound();

            khachHang.TrangThai = !khachHang.TrangThai;
            if (khachHang.TaiKhoan != null) khachHang.TaiKhoan.TrangThai = khachHang.TrangThai;

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Cập nhật trạng thái khách hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
