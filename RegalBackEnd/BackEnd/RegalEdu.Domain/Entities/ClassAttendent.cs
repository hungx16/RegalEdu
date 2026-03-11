using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Entities;

// Bảng lưu điểm danh học viên theo từng buổi học
[Table("ClassAttendent")]
public class ClassAttendent : BaseEntity
{
    public Guid ClassScheduleId { get; set; }
    [ForeignKey(nameof(ClassScheduleId))]
    public virtual ClassSchedule? ClassSchedule { get; set; }
    public Guid StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }
    //Trạng thái điểm danh 0 - Vắng mặt, 1 - có mặt
    public StudentParticipationStatus StudentParticipationStatus { get; set; } = StudentParticipationStatus.Absent;
    //Trạng thái làm bài tập: 0 - ko làm, 1 - có làm
    public StudentHomeworkStatus StudentHomeworkStatus { get; set; } = StudentHomeworkStatus.NotDone;
    public string? Comment { get; set; } // Nhận xét của giáo viên
    public double? ParticipationLevel { get; set; } // mức độ tham gia
    public double? LearningAbsorptionLevel { get; set; } // mức độ tiếp thu bài
    public double? DisciplineLevel { get; set; } // thời gian học tập & kỷ luật
    public double? Star { get; set; } // Số sao giáo viên đánh giá học viên
    public string? Attachment { get; set; } // File đính kèm (mảng attachment_id hoặc JSON)
    public string? Note { get; set; } // Ghi chú
    public double? HomeworkScore { get; set; }// Điểm bài tập về nhà
    public bool IsTuitionCalculated { get; set; } = false;// Đánh dấu buổi học này đã được tính trừ học phí hay chưa

}

