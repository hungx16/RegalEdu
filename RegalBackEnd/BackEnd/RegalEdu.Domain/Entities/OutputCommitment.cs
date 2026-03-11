using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table("OutputCommitment")]
    public class OutputCommitment : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string StudentCode { get; set; } = string.Empty;

        public string BeginningLevel { get; set; } = string.Empty;
        public string FinalLevel { get; set; } = string.Empty;
        public string OutputCommitmentInfo { get; set; } = string.Empty;

        // 9. Trạng thái
        public OutputCommitmentStatus OutputCommitmentStatus { get; set; } = OutputCommitmentStatus.NotFinished; // Đặt giá trị mặc định
        public int TotalRegisteredMonths { get; set; } // Tổng số tháng đã đăng ký
        [Column(TypeName = "decimal(18,2)")]
        public float TotalRegisteredFee { get; set; } // Tổng số tiền đã đóng
        // Tham chiếu đến Student
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}
