// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: ViewModel danh sách bàn ăn kèm điều kiện Tìm kiếm/Lọc/Sắp xếp/Phân trang (Module 2)
using QuanLyNhaHang_UNETI02_TI17A2ND.Common;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels
{
    public class BanAnListViewModel
    {
        public PagedList<BanAn> DanhSachBan { get; set; } = null!;

        // Giữ lại điều kiện Tìm kiếm/Lọc/Sắp xếp khi chuyển trang
        public string? TuKhoa { get; set; }
        public int? MaLoaiBan { get; set; }
        public int? SoChoNgoi { get; set; }
        public string? TrangThai { get; set; }
        public string? SapXep { get; set; }

        public List<LoaiBan> DanhSachLoaiBan { get; set; } = new();
    }
}
