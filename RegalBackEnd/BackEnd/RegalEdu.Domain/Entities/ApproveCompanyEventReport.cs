using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table ("ApproveCompanyEventReport")]
public class ApproveCompanyEventReport : BaseEntity
{
    [Required]
    [Column ("CompanyEventReportId")]
    public Guid? CompanyEventReportId { get; set; }

    public CompanyEventReport? CompanyEventReport { get; set; }

    [Column ("Reason")]
    public string? Reason { get; set; }

    [ForeignKey ("EmployeeId")]
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required]
    [Column ("ApproveStatus")]
    public CompanyEventProposalStatus ApproveStatus { get; set; }
}
