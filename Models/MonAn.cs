using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models
{
    // Entity: Món ăn
    // Ghi chú: CRUD chi tiết cho Món ăn thuộc trách nhiệm chính của Module 3.
    public class MonAn
    {
        [Key]
        public int MaMon { get; set; }

        [Required(ErrorMessage = "Tên món là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên món")]
        public string TenMon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại món là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Loại món")]
        public string LoaiMon { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000000, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        // true = đang phục vụ, false = ngừng phục vụ
        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;
    }
}
