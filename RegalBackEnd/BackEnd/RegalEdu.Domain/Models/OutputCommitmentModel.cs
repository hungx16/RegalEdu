using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class OutputCommitmentModel : BaseEntityModel
    {
        [Required]
        [MaxLength(50)]
        public string StudentCode { get; set; } = string.Empty;

        public string BeginningLevel { get; set; } = string.Empty;
        public string FinalLevel { get; set; } = string.Empty;
        public string OutputCommitmentInfo { get; set; } = string.Empty;

        // 9. Trạng thái
        public OutputCommitmentStatus OutputCommitmentStatus { get; set; } = OutputCommitmentStatus.NotFinished; // Đặt giá trị mặc định

        // Tham chiếu đến Student
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }

        public int TotalRegisteredMonths { get; set; } // Tổng số tháng đã đăng ký
        [Column(TypeName = "decimal(18,2)")]
        public float TotalRegisteredFee { get; set; } // Tổng số tiền đã đóng
    }
}
