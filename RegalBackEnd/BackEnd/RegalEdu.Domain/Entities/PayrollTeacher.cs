using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    public class PayrollTeacher : BaseEntity
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime SalaryMonth { get; set; } // Tháng lương (chỉ lưu tháng/năm)

        [Required]
        [Range(0, 31)]
        public int StandardWorkDay { get; set; } // Số ngày công chuẩn trong tháng

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 31)]
        public decimal ActualWorkDay { get; set; } // Số ngày công thực tế (có thể có phần thập phân)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal SalaryAmount { get; set; } // Tổng lương

        // Có thể thêm các trường bổ sung nếu cần
        [MaxLength(2000)]
        public string? Note { get; set; } // Ghi chú về bảng lương

        public bool IsPaid { get; set; } = false; // Đã thanh toán chưa

        [DataType(DataType.Date)]
        public DateTime? PaidDate { get; set; } // Ngày thanh toán
    }
}