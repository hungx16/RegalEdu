using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class RecruitmentApplyModel : BaseEntityModel
    {
        // 🔹 Họ tên ứng viên
        [Required]
        [Column ("CandidateName", TypeName = "nvarchar(255)")]
        public string CandidateName { get; set; } = null!;

        // 🔹 Email ứng viên
        [Required]
        [EmailAddress]
        [Column ("CandidateEmail", TypeName = "nvarchar(255)")]
        public string CandidateEmail { get; set; } = null!;

        // 🔹 Số điện thoại ứng viên
        [Required]
        [Phone]
        [Column ("CandidatePhone", TypeName = "nvarchar(20)")]
        public string CandidatePhone { get; set; } = null!;

        // 🔹 Kinh nghiệm của ứng viên
        [Column ("CandidateExperience", TypeName = "nvarchar(max)")]
        public string? CandidateExperience { get; set; }

        // 🔹 CV (tệp tài liệu, có thể lưu link hoặc tên file)
        [Column ("CandidateCV", TypeName = "nvarchar(1000)")]
        public string? CandidateCV { get; set; }

        // 🔹 Liên kết đến tin tuyển dụng (RecurementInfo)
        [Required]
        [Column ("RecruitmentId")]
        public Guid RecruitmentInfoId { get; set; }

        [ForeignKey (nameof (RecruitmentInfoId))]
        public virtual RecruitmentInfoModel? RecruitmentInfo { get; set; }

        public virtual AttachmentModel? Attachment { get; set; } // Navigation property đến Attachment
    }
}
