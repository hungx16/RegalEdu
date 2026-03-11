using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

// Model cho ClassSchedule - Buổi học của lớp
public class ClassScheduleModel : BaseEntityModel
{
    public Guid ClassId { get; set; }
    public virtual ClassModel? Class { get; set; }
    public Guid? TeacherId { get; set; }
    public virtual TeacherModel? Teacher { get; set; }

    public Guid? CourseLessonId { get; set; }
    public virtual CourseLessonModel? CourseLesson { get; set; }

    // Ngày diễn ra buổi học (chỉ ngày)
    [Column (TypeName = "date")]
    public DateTime Date { get; set; }

    // Thời gian bắt đầu (chỉ giờ, phút, giây)
    [Column (TypeName = "time")]
    public TimeSpan? StartTime { get; set; }

    // Thời gian kết thúc (chỉ giờ, phút, giây)
    [Column (TypeName = "time")]
    public TimeSpan? EndTime { get; set; }
    public byte DayOfWeek { get; set; }
    public ClassScheduleStatus ClassScheduleStatus { get; set; } = ClassScheduleStatus.NotStarted;
    public string? Content { get; set; }
    public SessionAttendanceStatus SessionAttendanceStatus { get; set; } = SessionAttendanceStatus.NotChecked;
    public Guid? Attender { get; set; }
    public string? Plan { get; set; }
    public string? HomeworkPlusName { get; set; }
    public string? HomeworkPlusContent { get; set; }
    public SessionAttendanceLockStatus SessionAttendanceLockStatus { get; set; } = SessionAttendanceLockStatus.Unlocked;

    public virtual AttachmentModel? Attachment { get; set; } // Navigation property đến Attachment
    //Hải bổ sung
    //public Guid? SubstituteTeacherId { get; set; }// Giáo viên dạy thay
    public string? CancelReason { get; set; }// Lý do

}