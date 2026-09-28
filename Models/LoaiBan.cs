using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Models
{
    // Entity: Loại bàn (VD: Bàn thường, Bàn VIP, Bàn ngoài trời...)
    public class LoaiBan
    {
        [Key]
        public int MaLoaiBan { get; set; }

        [Required(ErrorMessage = "Tên loại bàn là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên loại bàn tối đa 100 ký tự")]
        [Display(Name = "Tên loại bàn")]
        public string TenLoaiBan { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        // true = đang sử dụng, false = ngừng sử dụng
        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation property: 1 LoaiBan - n BanAn
        public ICollection<BanAn> DanhSachBanAn { get; set; } = new List<BanAn>();
    }
}
