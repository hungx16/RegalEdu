using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Models;

public class StudentCourseProgressModel
{
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public double ProgressPercent { get; set; }
    public Guid ClassScheduleId { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public ClassScheduleStatus ClassScheduleStatus { get; set; }
}
