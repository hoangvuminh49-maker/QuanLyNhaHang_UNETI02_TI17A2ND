// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Lớp hỗ trợ phân trang dùng chung cho danh sách bàn ăn (Module 2)
// Phân trang được thực hiện trực tiếp trên truy vấn Entity Framework Core (Skip/Take),
// không tải toàn bộ dữ liệu về rồi ẩn/hiện bằng JavaScript.
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Common
{
    public class PagedList<T> : List<T>
    {
        public int TrangHienTai { get; private set; }
        public int TongSoTrang { get; private set; }
        public int TongSoDong { get; private set; }

        public PagedList(List<T> items, int tongSoDong, int trangHienTai, int soDongMoiTrang)
        {
            TrangHienTai = trangHienTai;
            TongSoDong = tongSoDong;
            TongSoTrang = (int)Math.Ceiling(tongSoDong / (double)soDongMoiTrang);
            if (TongSoTrang == 0) TongSoTrang = 1;
            AddRange(items);
        }

        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int trangHienTai, int soDongMoiTrang)
        {
            if (trangHienTai < 1) trangHienTai = 1;

            var tongSoDong = await source.CountAsync();
            var items = await source
                .Skip((trangHienTai - 1) * soDongMoiTrang)
                .Take(soDongMoiTrang)
                .ToListAsync();

            return new PagedList<T>(items, tongSoDong, trangHienTai, soDongMoiTrang);
        }
    }
}
