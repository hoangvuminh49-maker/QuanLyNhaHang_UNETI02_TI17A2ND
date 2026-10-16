// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Entity Bàn ăn (Module 2 - Quản lý, tìm kiếm, lọc, sắp xếp, phân trang bàn ăn)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models
{
    public class BanAn
    {
        [Key]
        public int MaBan { get; set; }

        [Required(ErrorMessage = "Tên bàn không được để trống")]
        [StringLength(50, ErrorMessage = "Tên bàn tối đa 50 ký tự")]
        [Display(Name = "Tên bàn")]
        public string TenBan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại bàn")]
        [Display(Name = "Loại bàn")]
        public int MaLoaiBan { get; set; }

        [ForeignKey(nameof(MaLoaiBan))]
        public LoaiBan? LoaiBan { get; set; }

        [Range(1, 100, ErrorMessage = "Số chỗ ngồi phải lớn hơn 0")]
        [Display(Name = "Số chỗ ngồi")]
        public int SoChoNgoi { get; set; }

        [StringLength(150)]
        [Display(Name = "Vị trí")]
        public string? ViTri { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = Common.TrangThaiBan.SanSang;

        public ICollection<PhieuDatBan>? PhieuDatBans { get; set; }
    }
}
