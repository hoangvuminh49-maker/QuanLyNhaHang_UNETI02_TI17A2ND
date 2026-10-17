// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: Entity Món ăn (Module 3 - Quản lý thực đơn)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Models
{
    public class MonAn
    {
        [Key]
        public int MaMon { get; set; }

        [Required(ErrorMessage = "Tên món không được để trống")]
        [StringLength(150)]
        [Display(Name = "Tên món")]
        public string TenMon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại món không được để trống")]
        [StringLength(50)]
        [Display(Name = "Loại món")]
        public string LoaiMon { get; set; } = string.Empty;

        [Range(0.01, 100000000, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Đang phục vụ")]
        public bool TrangThai { get; set; } = true;
    }
}
