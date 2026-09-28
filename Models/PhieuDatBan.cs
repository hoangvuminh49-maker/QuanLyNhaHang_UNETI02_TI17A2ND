using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    // Entity: Phiếu đặt bàn
    // Ghi chú: Nghiệp vụ đặt bàn / kiểm tra trùng lịch thuộc Module 3;
    // xác nhận/hủy/dashboard/thống kê thuộc Module 4.
    public class PhieuDatBan
    {
        [Key]
        public int MaPhieu { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }

        [Required]
        public int MaBan { get; set; }

        [ForeignKey(nameof(MaBan))]
        public BanAn? BanAn { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Ngày sử dụng là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime NgaySuDung { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Số người phải lớn hơn 0")]
        [Display(Name = "Số người")]
        public int SoNguoi { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.ChoXacNhan;
    }
}
