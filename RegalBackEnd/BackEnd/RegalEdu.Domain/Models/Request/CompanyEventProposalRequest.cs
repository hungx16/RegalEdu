
namespace RegalEdu.Domain.Models.Request;

public class CompanyEventProposalRequest
{
    public required CompanyEventModel CompanyEvent { get; set; }
    public List<EventPublicationModel>? Publications { get; set; }
    public List<EventCashModel>? CashCosts { get; set; }
    public List<EventParticipantModel>? Participants { get; set; }

    public List<AttachmentModel>? Attachments { get; set; }
    public int ProposalQuantity { get; set; }
    public List<string>? DeletedAttachmentIds { get; set; }
    public List<string>? DeletedParticipantIds { get; set; }
    public List<string>? DeletedCashIds { get; set; }
    public List<string>? DeletedPublicationIds { get; set; }
}