// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Entity Tài khoản phục vụ đăng nhập và phân quyền (Module 1)
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Mật khẩu từ 4 đến 100 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò phải được xác định")]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = Common.VaiTro.KhachHang;

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        public KhachHang? KhachHang { get; set; }
    }
}
