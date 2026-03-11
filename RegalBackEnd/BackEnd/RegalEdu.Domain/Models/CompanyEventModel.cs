using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table("CompanyEvent")]
public class CompanyEventModel : BaseEntityModel
{
    public Guid? AllocationDetailEventId { get; set; }
    public virtual AllocationDetailEventModel? AllocationDetailEvent { get; set; }
    public string CompanyEventCode { get; set; } = string.Empty;
    public string CompanyEventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public Guid? AffiliatePartnerId { get; set; }
    public virtual AffiliatePartnerModel? AffiliatePartner { get; set; }
    public int? NumberStudents { get; set; }
    public string? Propose { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal? TotalAmount { get; set; }
    public EventSize EventSize { get; set; } // Enum cần định nghĩa riêng

    public CompanyEventProposalStatus CompanyEventStatus { get; set; } // Enum cần định nghĩa riêng

    public virtual List<EventCashModel>? EventCashes { get; set; }
    public virtual List<EventPublicationModel>? EventPublications { get; set; }

    public virtual List<EventParticipantModel>? EventParticipants { get; set; }

    public virtual List<AttachmentModel>? Attachments { get; set; }

    public virtual List<ApproveCompanyEventModel>? ApproveCompanyEvents { get; set; }

}
