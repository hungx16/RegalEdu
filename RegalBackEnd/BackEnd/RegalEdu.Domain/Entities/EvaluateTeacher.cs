using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    // Lưu đánh giá của học viên đối với giáo viên
    public class EvaluateTeacher : BaseEntity
    {
        [Required]
        public Guid TeacherId { get; set; }

        [MaxLength(1000)]
        public string? EvaluateName { get; set; }

        [MaxLength(1000)]
        public string? EvaluateNick { get; set; }

        public DateTime EvaluateDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? EvaluateType { get; set; }

        // Đánh giá của học viên
        public double? StarRating { get; set; }

        [MaxLength(1000)]
        public string? ResponseContent { get; set; }

        // Liên kết
        public Guid? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        public Guid? ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public Class? Class { get; set; }

        public Guid? ClassScheduleId { get; set; }
        [ForeignKey(nameof(ClassScheduleId))]
        public ClassSchedule? ClassSchedule { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;
    }
}
