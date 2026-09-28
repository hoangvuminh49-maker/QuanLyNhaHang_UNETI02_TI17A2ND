using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    // Entity: Bàn ăn
    // Ghi chú: Entity này được khai báo đầy đủ ngay từ đầu để Migration tạo đúng
    // toàn bộ lược đồ CSDL. Chức năng CRUD/Tìm kiếm/Lọc/Sắp xếp/Phân trang
    // cho Bàn ăn thuộc trách nhiệm chính của Thành viên phụ trách Module 2.
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        [Required(ErrorMessage = "Tên bàn là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Tên bàn")]
        public string TenBan { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại bàn")]
        public int MaLoaiBan { get; set; }

        [ForeignKey(nameof(MaLoaiBan))]
        public LoaiBan? LoaiBan { get; set; }

        [Required(ErrorMessage = "Số chỗ ngồi là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0")]
        [Display(Name = "Số chỗ ngồi")]
        public int SoChoNgoi { get; set; }

        [StringLength(100)]
        [Display(Name = "Vị trí")]
        public string? ViTri { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiBan TrangThai { get; set; } = TrangThaiBan.SanSang;

        // Navigation property: 1 BanAn - n PhieuDatBan
        public ICollection<PhieuDatBan> DanhSachPhieuDatBan { get; set; } = new List<PhieuDatBan>();
    }
}
