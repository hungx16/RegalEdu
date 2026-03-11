using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table("CompanyEventReport")]
public class CompanyEventReportModel : BaseEntityModel
{
    public Guid CompanyEventId { get; set; }
    public CompanyEventModel? CompanyEvent { get; set; }

    public DateTime EventDate { get; set; }
    public string? CompanyEventReportCode { get; set; }

    public int? NumberStudents { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }


    public CompanyEventProposalStatus? CompanyEventStatus { get; set; }

    public string? LinkContent { get; set; }
    public string? LinkFanpage { get; set; }

    private List<EventPublicationModel>? _eventPublications;
    private List<EventCashModel>? _eventCashes;
    private List<EventParticipantModel>? _eventParticipants;

    public virtual List<EventPublicationModel>? EventPublications
    {
        get => _eventPublications;
        set => _eventPublications = value;
    }

    public virtual List<EventPublicationModel>? Publications
    {
        get => EventPublications;
        set => EventPublications = value;
    }

    public virtual List<EventCashModel>? EventCashes
    {
        get => _eventCashes;
        set => _eventCashes = value;
    }

    public virtual List<EventCashModel>? CashCosts
    {
        get => EventCashes;
        set => EventCashes = value;
    }

    public virtual List<EventParticipantModel>? EventParticipants
    {
        get => _eventParticipants;
        set => _eventParticipants = value;
    }

    public virtual List<EventParticipantModel>? Participants
    {
        get => EventParticipants;
        set => EventParticipants = value;
    }

    public virtual List<AttachmentModel>? Attachments { get; set; }

    public virtual List<ApproveCompanyEventReportModel>? ApproveCompanyEvents { get; set; }
}
