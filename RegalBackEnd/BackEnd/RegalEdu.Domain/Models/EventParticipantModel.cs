using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table("EventParticipant")]
public class EventParticipantModel : BaseEntityModel
{
    public Guid? CompanyEventId { get; set; }
    public Guid? CompanyEventReportId { get; set; }
    public string? StudentCode { get; set; }

    public bool IsStudent { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? ParticipantGender { get; set; }
    public DateTime? ParticipantDateOfBirth { get; set; }
    public string? ParticipantAddress { get; set; }
    public string? ParticipantPhoneNumber { get; set; }
    public string? ParticipantContact { get; set; }
    public string? ParticipantEmail { get; set; }
    public string? ParticipantSchool { get; set; }
    public string? ParticipantSourceKnown { get; set; }
    public string? ParticipantJob { get; set; }
    public Guid? EmployeeId { get; set; }
    public EmployeeModel? Employee { get; set; }
}
