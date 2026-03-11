using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class EvaluateTeacherModel : BaseEntityModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        public TeacherModel? Teacher { get; set; }

        public string? EvaluateName { get; set; }

        public string? EvaluateNick { get; set; }

        public DateTime EvaluateDate { get; set; } = DateTime.UtcNow;

        public string? EvaluateType { get; set; }

        // Đánh giá của học viên
        public double? StarRating { get; set; }

        public string? ResponseContent { get; set; }

        public Guid? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? ClassScheduleId { get; set; }
    }
}
