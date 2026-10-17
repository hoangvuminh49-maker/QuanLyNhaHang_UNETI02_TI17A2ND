// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: Controller Quản lý món ăn và Thực đơn (Module 3)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    public class MonAnController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonAnController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AdminOnly]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonAns.OrderBy(m => m.LoaiMon).ThenBy(m => m.TenMon).ToListAsync());
        }

        [AdminOnly]
        public async Task<IActionResult> Details(int id)
        {
            var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null) return NotFound();
            return View(monAn);
        }

        [AdminOnly]
        public IActionResult Create() => View();

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonAn model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Thêm món ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null) return NotFound();
            return View(monAn);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonAn model)
        {
            if (id != model.MaMon) return NotFound();
            if (!ModelState.IsValid) return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MonAns.AnyAsync(m => m.MaMon == id)) return NotFound();
                throw;
            }

            TempData["ThongBao"] = "Cập nhật món ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        public async Task<IActionResult> Delete(int id)
        {
            var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null) return NotFound();
            return View(monAn);
        }

        [AdminOnly]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null) return NotFound();

            _context.MonAns.Remove(monAn);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Xóa món ăn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null) return NotFound();

            monAn.TrangThai = !monAn.TrangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Khách hàng xem thực đơn - chỉ hiển thị món đang phục vụ
        public async Task<IActionResult> ThucDon()
        {
            var thucDon = await _context.MonAns
                .Where(m => m.TrangThai)
                .OrderBy(m => m.LoaiMon).ThenBy(m => m.TenMon)
                .ToListAsync();
            return View(thucDon);
        }
    }
}
