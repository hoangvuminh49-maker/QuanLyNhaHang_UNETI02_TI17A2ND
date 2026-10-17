// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Controller Quản lý loại bàn - CRUD và cập nhật trạng thái (Module 1)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Data;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Controllers
{
    [AdminOnly]
    public class LoaiBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.LoaiBans.OrderBy(l => l.TenLoaiBan).ToListAsync();
            return View(danhSach);
        }

        public async Task<IActionResult> Details(int id)
        {
            var loaiBan = await _context.LoaiBans.FirstOrDefaultAsync(l => l.MaLoaiBan == id);
            if (loaiBan == null) return NotFound();
            return View(loaiBan);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiBan model)
        {
            if (await _context.LoaiBans.AnyAsync(l => l.TenLoaiBan == model.TenLoaiBan))
                ModelState.AddModelError(nameof(model.TenLoaiBan), "Tên loại bàn đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Thêm loại bàn thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var loaiBan = await _context.LoaiBans.FindAsync(id);
            if (loaiBan == null) return NotFound();
            return View(loaiBan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiBan model)
        {
            if (id != model.MaLoaiBan) return NotFound();

            if (await _context.LoaiBans.AnyAsync(l => l.TenLoaiBan == model.TenLoaiBan && l.MaLoaiBan != id))
                ModelState.AddModelError(nameof(model.TenLoaiBan), "Tên loại bàn đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LoaiBans.AnyAsync(l => l.MaLoaiBan == id)) return NotFound();
                throw;
            }

            TempData["ThongBao"] = "Cập nhật loại bàn thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var loaiBan = await _context.LoaiBans.FirstOrDefaultAsync(l => l.MaLoaiBan == id);
            if (loaiBan == null) return NotFound();
            return View(loaiBan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiBan = await _context.LoaiBans.FindAsync(id);
            if (loaiBan == null) return NotFound();

            // Không được xóa loại bàn nếu còn Bàn ăn thuộc loại này
            bool conBanAn = await _context.BanAns.AnyAsync(b => b.MaLoaiBan == id);
            if (conBanAn)
            {
                TempData["Loi"] = "Không thể xóa loại bàn vì vẫn còn bàn ăn thuộc loại này.";
                return RedirectToAction(nameof(Index));
            }

            _context.LoaiBans.Remove(loaiBan);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Xóa loại bàn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            var loaiBan = await _context.LoaiBans.FindAsync(id);
            if (loaiBan == null) return NotFound();

            loaiBan.TrangThai = !loaiBan.TrangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
