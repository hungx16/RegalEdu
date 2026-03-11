using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("Attachment")]
public class Attachment : BaseEntity
{
    public required string FileName { get; set; }
    public required string Path { get; set; }
    public Guid? SupportingDocumentId { get; set; }
    [ForeignKey("SupportingDocumentId")]
    public SupportingDocument? SupportingDocument { get; set; }
    public Guid? LectureTypeId { get; set; }
    [ForeignKey("LectureTypeId")]
    public LectureType? LectureType { get; set; }

    public Guid? RecruitmentApplyId { get; set; }
    [ForeignKey("RecruitmentApplyId")]
    public RecruitmentApply? RecruitmentApply { get; set; }

    public Guid? CompanyEventId { get; set; }

    [ForeignKey("CompanyEventId")]
    public CompanyEvent? CompanyEvent { get; set; }

    [ForeignKey("ClassScheduleId")]
    public Guid? ClassScheduleId { get; set; }

    public ClassSchedule? ClassSchedule { get; set; }

    public Guid? CourseLessonHomeworkId { get; set; }
    [ForeignKey("CourseLessonHomeworkId")]
    public CourseLesson? CourseLessonHomework { get; set; }

    public Guid? CourseLessonReferenceId { get; set; }
    [ForeignKey("CourseLessonReferenceId")]
    public CourseLesson? CourseLessonReference { get; set; }

    public Guid? CompanyEventReportId { get; set; }

    [ForeignKey("CompanyEventReportId")]
    public CompanyEventReport? CompanyEventReport { get; set; }
}
