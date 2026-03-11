using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("CompanyEvent")]
public class CompanyEvent : BaseEntity
{
    public Guid? AllocationDetailEventId { get; set; }
    public virtual AllocationDetailEvent? AllocationDetailEvent { get; set; }
    public string CompanyEventCode { get; set; } = string.Empty;
    public string CompanyEventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public Guid? AffiliatePartnerId { get; set; }
    public virtual AffiliatePartner? AffiliatePartner { get; set; }
    public int? NumberStudents { get; set; }
    public string? Propose { get; set; }
    [Column(TypeName = "decimal(18,2)")]

    public decimal? TotalAmount { get; set; }
    public EventSize EventSize { get; set; } // Enum cần định nghĩa riêng

    public CompanyEventProposalStatus CompanyEventStatus { get; set; } // Enum cần định nghĩa riêng

    public virtual List<EventCash>? EventCashes { get; set; }
    public virtual List<EventPublication>? EventPublications { get; set; }

    public virtual List<EventParticipant>? EventParticipants { get; set; }

    public virtual List<Attachment>? Attachments { get; set; }

    public virtual List<ApproveCompanyEvent>? ApproveCompanyEvents { get; set; }
    public virtual List<CompanyEventReport>? CompanyEventReports { get; set; }

}
