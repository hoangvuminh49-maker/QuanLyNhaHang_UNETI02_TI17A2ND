// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: ViewModel đặt bàn của khách hàng (Module 3)
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models.ViewModels
{
    public class DangKyDatBanViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bàn")]
        [Display(Name = "Bàn ăn")]
        public int MaBan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sử dụng")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime NgaySuDung { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; }

        [Range(1, 100, ErrorMessage = "Số người phải lớn hơn 0")]
        [Display(Name = "Số người")]
        public int SoNguoi { get; set; } = 2;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
