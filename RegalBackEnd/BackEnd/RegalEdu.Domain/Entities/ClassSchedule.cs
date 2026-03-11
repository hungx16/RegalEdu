using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
//Bảng các buổi học của lớp
[Table ("ClassSchedules")]
public class ClassSchedule : BaseEntity
{
    public Guid ClassId { get; set; }
    [ForeignKey (nameof (ClassId))]
    public virtual Class? Class { get; set; }
    public Guid? TeacherId { get; set; }
    [ForeignKey (nameof (TeacherId))]
    public virtual Teacher? Teacher { get; set; }
    public Guid? CourseLessonId { get; set; }
    [ForeignKey (nameof (CourseLessonId))]
    public virtual CourseLesson? CourseLesson { get; set; }
    [Column (TypeName = "date")]
    public DateTime Date { get; set; }
    [Column (TypeName = "time")]
    public TimeSpan? StartTime { get; set; }

    [Column (TypeName = "time")]
    public TimeSpan? EndTime { get; set; }
    public byte DayOfWeek { get; set; }
    //Trạng thái buổi học: 0 - Chưa học, 1 - Hoàn thành (ko được hủy), 2 - Hủy
    public ClassScheduleStatus ClassScheduleStatus { get; set; } = ClassScheduleStatus.NotStarted;
    public string? Content { get; set; }
    //Trạng thái điểm danh: 0 - Chưa điểm danh, 1 - Đã điểm danh, 2 - Đã xác nhận
    public SessionAttendanceStatus SessionAttendanceStatus { get; set; } = SessionAttendanceStatus.NotChecked;    
    public string? Plan { get; set; }// Giáo án, mảng các attachment_id
    public string? HomeworkPlusName { get; set; }
    public string? HomeworkPlusContent { get; set; } // BTVN bổ sung, mảng các attachment_id    
    //Trạng thái khóa điểm danh 0 - Ko khóa, 1 - Khóa - Mặc định là khóa điểm danh
    public SessionAttendanceLockStatus SessionAttendanceLockStatus { get; set; } = SessionAttendanceLockStatus.Unlocked;
    public virtual ICollection<ClassAttendent>? ClassAttendants { get; set; }

    public virtual Attachment? Attachment { get; set; } // Navigation property đến Attachment
    //Hải bổ sung
    //public Guid? SubstituteTeacherId { get; set; }// Giáo viên dạy thay
    public string? CancelReason { get; set; }// Lý do


}
