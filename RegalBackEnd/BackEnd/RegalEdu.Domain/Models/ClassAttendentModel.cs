using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Models
{
    // Model lưu thông tin điểm danh học viên theo từng buổi học
    public class ClassAttendentModel : BaseEntityModel
    {
        public Guid ClassScheduleId { get; set; }
        public ClassSchedule? ClassSchedule { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public StudentParticipationStatus StudentParticipationStatus { get; set; }
            = StudentParticipationStatus.Absent;
        public StudentHomeworkStatus StudentHomeworkStatus { get; set; }
            = StudentHomeworkStatus.NotDone;
        public string? Comment { get; set; }
        public double? ParticipationLevel { get; set; }
        public double? LearningAbsorptionLevel { get; set; }
        public double? DisciplineLevel { get; set; }
        public double? Star { get; set; }
        public string? Attachment { get; set; }
        public string? Note { get; set; }
        public double? HomeworkScore { get; set; }

        public bool IsTuitionCalculated { get; set; } = false;// Đánh dấu buổi học này đã được tính trừ học phí hay chưa

    }
}

