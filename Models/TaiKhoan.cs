using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang.Models
{
    // Entity: Tài khoản đăng nhập hệ thống (Admin hoặc Khách hàng)
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Mật khẩu tối thiểu 4 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vai trò")]
        public VaiTro VaiTro { get; set; }

        // true = đang hoạt động, false = bị khóa
        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation property: 1 TaiKhoan - 1 KhachHang (chỉ áp dụng khi VaiTro = KhachHang)
        public KhachHang? KhachHang { get; set; }
    }
}
