// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Entity Loại bàn (Module 1 - Quản lý loại bàn)
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models
{
    public class LoaiBan
    {
        [Key]
        public int MaLoaiBan { get; set; }

        [Required(ErrorMessage = "Tên loại bàn không được để trống")]
        [StringLength(100, ErrorMessage = "Tên loại bàn tối đa 100 ký tự")]
        [Display(Name = "Tên loại bàn")]
        public string TenLoaiBan { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        [Display(Name = "Đang sử dụng")]
        public bool TrangThai { get; set; } = true;

        public ICollection<BanAn>? BanAns { get; set; }
    }
}
