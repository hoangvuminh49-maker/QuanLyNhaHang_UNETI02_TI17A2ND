using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    // Entity: Khách hàng
    // Ghi chú: CRUD/nghiệp vụ chi tiết cho Khách hàng thuộc trách nhiệm Module 3.
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        // Quan hệ 1-1 với TaiKhoan
        [Required]
        public int MaTaiKhoan { get; set; }

        [ForeignKey(nameof(MaTaiKhoan))]
        public TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation property: 1 KhachHang - n PhieuDatBan
        public ICollection<PhieuDatBan> DanhSachPhieuDatBan { get; set; } = new List<PhieuDatBan>();
    }
}
