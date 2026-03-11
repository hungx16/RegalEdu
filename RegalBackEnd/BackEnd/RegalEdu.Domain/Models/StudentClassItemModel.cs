using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Models;

public class StudentClassItemModel
{
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public string? ClassCode { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    public ClassStatus ClassStatus { get; set; }
    public ClassMethod Method { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public StudentCourseStatus? StudentCourseStatus { get; set; }
    public PaymentStatus? PaymentCourseStatus { get; set; }
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public double ProgressPercent { get; set; }
    public Guid? NextClassScheduleId { get; set; }
    public DateTime? NextSessionDate { get; set; }
    public TimeSpan? NextSessionStartTime { get; set; }
    public TimeSpan? NextSessionEndTime { get; set; }
    public ClassScheduleStatus? NextClassScheduleStatus { get; set; }
    public StudentParticipationStatus? NextStudentParticipationStatus { get; set; }
}
