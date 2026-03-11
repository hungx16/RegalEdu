using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("CompanyEventReport")]
public class CompanyEventReport : BaseEntity
{
    [Required]
    [Column("CompanyEventId")]
    public Guid CompanyEventId { get; set; }
    public CompanyEvent? CompanyEvent { get; set; }

    public DateTime EventDate { get; set; }

    public string? CompanyEventReportCode { get; set; }
    public int? NumberStudents { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }


    public CompanyEventProposalStatus? CompanyEventStatus { get; set; }

    public string? LinkContent { get; set; }
    public string? LinkFanpage { get; set; }
    public virtual List<EventCash>? EventCashes { get; set; }
    public virtual List<EventPublication>? EventPublications { get; set; }

    public virtual List<EventParticipant>? EventParticipants { get; set; }

    public virtual List<Attachment>? Attachments { get; set; }
    public virtual List<ApproveCompanyEventReport>? ApproveCompanyEvents { get; set; }
}
