namespace RegalEdu.Domain.Models;

public class AttachmentModel : BaseEntityModel
{
    public required string FileName { get; set; }
    public required string Path { get; set; }
    public Guid? SupportingDocumentId { get; set; }

    public Guid? LectureTypeId { get; set; }


    public Guid? CompanyEventId { get; set; }


    public Guid? ClassScheduleId { get; set; }

    public Guid? CourseLessonHomeworkId { get; set; }

    public Guid? CourseLessonReferenceId { get; set; }
    public Guid? CompanyEventReportId { get; set; }
}
