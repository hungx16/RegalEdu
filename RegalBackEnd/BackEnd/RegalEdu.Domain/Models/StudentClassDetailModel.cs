using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Models;

public class StudentClassDetailModel
{
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public string? ClassCode { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public ClassMethod Method { get; set; }
    public string? ScheduleText { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ClassStatus ClassStatus { get; set; }
    public int TotalSessions { get; set; }
    public int AttendedSessions { get; set; }
    public double ProgressPercent { get; set; }
    public List<StudentClassSessionModel> Sessions { get; set; } = new();
    public List<StudentHomeworkItemModel> Homeworks { get; set; } = new();
    public List<StudentClassMaterialModel> Materials { get; set; } = new();
    public List<StudentClassCommentModel> Comments { get; set; } = new();
    public List<StudentTeacherEvaluationModel> StudentTeacherEvaluations { get; set; } = new();
}

public class StudentClassSessionModel
{
    public Guid ClassScheduleId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? LessonName { get; set; }
    public string? SessionName { get; set; }
    public string? Objective { get; set; }
    public ClassScheduleStatus ClassScheduleStatus { get; set; }
    public StudentParticipationStatus? StudentParticipationStatus { get; set; }
    public StudentHomeworkStatus? StudentHomeworkStatus { get; set; }
    public double? HomeworkScore { get; set; }
    public string? HomeworkTitle { get; set; }
    public string? HomeworkDescription { get; set; }
    public List<AttachmentModel> Attachments { get; set; } = new();
}

public class StudentHomeworkItemModel
{
    public Guid ClassScheduleId { get; set; }
    public DateTime Date { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public StudentHomeworkStatus? Status { get; set; }
    public double? Score { get; set; }
    public ClassScheduleStatus ClassScheduleStatus { get; set; }
}

public class StudentClassMaterialModel
{
    public Guid AttachmentId { get; set; }
    public Guid ClassScheduleId { get; set; }
    public string? FileName { get; set; }
    public string? Path { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StudentClassCommentModel
{
    public Guid ClassScheduleId { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public double? Star { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public double? ParticipationLevel { get; set; } // mức độ tham gia
    public double? LearningAbsorptionLevel { get; set; } // mức độ tiếp thu bài
    public double? DisciplineLevel { get; set; } // thời gian học tập & kỷ luật


}

public class StudentTeacherEvaluationModel
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public Guid ClassId { get; set; }
    public Guid ClassScheduleId { get; set; }
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public double? StarRating { get; set; }
    public string? ResponseContent { get; set; }
    public DateTime EvaluateDate { get; set; }
}
