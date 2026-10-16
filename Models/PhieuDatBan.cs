// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: Entity Phiếu đặt bàn (Module 3 - Đặt bàn của khách hàng)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models
{
    public class PhieuDatBan
    {
        [Key]
        public int MaPhieu { get; set; }

        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }

        [Display(Name = "Bàn ăn")]
        public int MaBan { get; set; }

        [ForeignKey(nameof(MaBan))]
        public BanAn? BanAn { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime NgaySuDung { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; }

        [Range(1, 100, ErrorMessage = "Số người phải lớn hơn 0")]
        [Display(Name = "Số người")]
        public int SoNguoi { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = Common.TrangThaiPhieu.ChoXacNhan;
    }
}
