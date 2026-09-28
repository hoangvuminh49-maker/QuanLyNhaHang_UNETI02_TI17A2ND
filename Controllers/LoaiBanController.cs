// Họ và tên: [Điền tên Thành viên 1]
// Mã sinh viên: [Điền MSSV]
// Nội dung thực hiện: Quản lý Loại bàn (CRUD)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Filters;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Controllers
{
    [SessionAuthorize(VaiTro.Admin)] // Toàn bộ controller chỉ Admin được truy cập
    public class LoaiBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /LoaiBan
        public async Task<IActionResult> Index(string? tuKhoa)
        {
            var query = _context.LoaiBans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(l => l.TenLoaiBan.Contains(tuKhoa));
            }

            var danhSach = await query
                .OrderBy(l => l.TenLoaiBan)
                .ToListAsync();

            ViewBag.TuKhoa = tuKhoa;
            return View(danhSach);
        }

        // GET: /LoaiBan/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var loaiBan = await _context.LoaiBans
                .Include(l => l.DanhSachBanAn)
                .FirstOrDefaultAsync(l => l.MaLoaiBan == id);

            if (loaiBan == null) return NotFound();
            return View(loaiBan);
        }

        // GET: /LoaiBan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /LoaiBan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiBan model)
        {
            bool trungTen = await _context.LoaiBans
                .AnyAsync(l => l.TenLoaiBan == model.TenLoaiBan);
            if (trungTen)
            {
                ModelState.AddModelError(nameof(model.TenLoaiBan), "Tên loại bàn đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.LoaiBans.Add(model);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Thêm loại bàn thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /LoaiBan/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var loaiBan = await _context.LoaiBans.FindAsync(id);
            if (loaiBan == null) return NotFound();
            return View(loaiBan);
        }

        // POST: /LoaiBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiBan model)
        {
            if (id != model.MaLoaiBan) return NotFound();

            bool trungTen = await _context.LoaiBans
                .AnyAsync(l => l.TenLoaiBan == model.TenLoaiBan && l.MaLoaiBan != id);
            if (trungTen)
            {
                ModelState.AddModelError(nameof(model.TenLoaiBan), "Tên loại bàn đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _context.LoaiBans.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Cập nhật loại bàn thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LoaiBans.AnyAsync(l => l.MaLoaiBan == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /LoaiBan/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var loaiBan = await _context.LoaiBans
                .Include(l => l.DanhSachBanAn)
                .FirstOrDefaultAsync(l => l.MaLoaiBan == id);

            if (loaiBan == null) return NotFound();

            // Cảnh báo trước cho người dùng nếu loại bàn đang có bàn ăn sử dụng
            ViewBag.DangDuocSuDung = loaiBan.DanhSachBanAn.Any();
            return View(loaiBan);
        }

        // POST: /LoaiBan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiBan = await _context.LoaiBans
                .Include(l => l.DanhSachBanAn)
                .FirstOrDefaultAsync(l => l.MaLoaiBan == id);

            if (loaiBan == null) return NotFound();

            // Nghiệp vụ bắt buộc: không cho xóa Loại bàn nếu đang có Bàn ăn thuộc loại này
            if (loaiBan.DanhSachBanAn.Any())
            {
                TempData["Loi"] = $"Không thể xóa \"{loaiBan.TenLoaiBan}\" vì đang có {loaiBan.DanhSachBanAn.Count} bàn ăn thuộc loại này.";
                return RedirectToAction(nameof(Index));
            }

            _context.LoaiBans.Remove(loaiBan);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Xóa loại bàn thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /LoaiBan/DoiTrangThai/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var loaiBan = await _context.LoaiBans.FindAsync(id);
            if (loaiBan == null) return NotFound();

            loaiBan.TrangThai = !loaiBan.TrangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
