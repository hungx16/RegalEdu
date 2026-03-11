using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Models;

public class StudentTimetableItemModel
{
    public Guid ClassScheduleId { get; set; }
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public ClassScheduleStatus ClassScheduleStatus { get; set; }
    public StudentParticipationStatus? StudentParticipationStatus { get; set; }
}
