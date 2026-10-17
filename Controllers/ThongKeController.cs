// Họ và tên: Nguyễn Huy Hoàng 
// Mã sinh viên: 22203100058
// Nội dung thực hiện: Controller Dashboard và Thống kê hệ thống (Module 4)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    [AdminOnly]
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var vm = new DashboardViewModel
            {
                TongSoLoaiBan = await _context.LoaiBans.CountAsync(),
                TongSoBanAn = await _context.BanAns.CountAsync(),
                TongSoMonAn = await _context.MonAns.CountAsync(),
                TongSoKhachHang = await _context.KhachHangs.CountAsync(),
                TongSoPhieu = await _context.PhieuDatBans.CountAsync(),
                SoPhieuChoXacNhan = await _context.PhieuDatBans.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoXacNhan),
                SoPhieuDaXacNhan = await _context.PhieuDatBans.CountAsync(p => p.TrangThai == TrangThaiPhieu.DaXacNhan),
                SoBanDangSuDung = await _context.BanAns.CountAsync(b => b.TrangThai == TrangThaiBan.DangSuDung)
            };
            return View(vm);
        }

        public async Task<IActionResult> ThongKe()
        {
            // Số bàn theo từng loại
            var soBanTheoLoai = await _context.BanAns
                .Include(b => b.LoaiBan)
                .GroupBy(b => b.LoaiBan!.TenLoaiBan)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // Số bàn theo từng trạng thái
            var soBanTheoTrangThai = await _context.BanAns
                .GroupBy(b => b.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // Số phiếu đặt theo từng trạng thái
            var soPhieuTheoTrangThai = await _context.PhieuDatBans
                .GroupBy(p => p.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // Số lượt đặt theo từng bàn + Bàn có số lượt đặt nhiều nhất
            var soLuotTheoBan = await _context.PhieuDatBans
                .Include(p => p.BanAn)
                .GroupBy(p => p.BanAn!.TenBan)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Số lượt đặt theo từng loại bàn
            var soLuotTheoLoaiBan = await _context.PhieuDatBans
                .Include(p => p.BanAn)!.ThenInclude(b => b!.LoaiBan)
                .GroupBy(p => p.BanAn!.LoaiBan!.TenLoaiBan)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Số lượt đặt theo từng khách hàng
            var soLuotTheoKhachHang = await _context.PhieuDatBans
                .Include(p => p.KhachHang)
                .GroupBy(p => p.KhachHang!.HoTen)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // Số lượt đặt theo tháng
            var soLuotTheoThang = await _context.PhieuDatBans
                .GroupBy(p => new { p.NgaySuDung.Year, p.NgaySuDung.Month })
                .Select(g => new { Nam = g.Key.Year, Thang = g.Key.Month, SoLuong = g.Count() })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToListAsync();

            ViewBag.SoBanTheoLoai = soBanTheoLoai;
            ViewBag.SoBanTheoTrangThai = soBanTheoTrangThai;
            ViewBag.SoPhieuTheoTrangThai = soPhieuTheoTrangThai;
            ViewBag.SoLuotTheoBan = soLuotTheoBan;
            ViewBag.BanNhieuLuotNhat = soLuotTheoBan.FirstOrDefault();
            ViewBag.SoLuotTheoLoaiBan = soLuotTheoLoaiBan;
            ViewBag.SoLuotTheoKhachHang = soLuotTheoKhachHang;
            ViewBag.SoLuotTheoThang = soLuotTheoThang;

            return View();
        }
    }
}
