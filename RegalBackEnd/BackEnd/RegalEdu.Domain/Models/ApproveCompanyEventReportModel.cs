using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

[Table ("ApproveCompanyEventReport")]
public class ApproveCompanyEventReportModel : BaseEntityModel
{
    [Required]
    [Column ("CompanyEventReportId")]
    public Guid? CompanyEventReportId { get; set; }

    public CompanyEventReportModel? CompanyEventReport { get; set; }

    [Column ("Reason")]
    public string? Reason { get; set; }

    public Guid? EmployeeId { get; set; }
    public EmployeeModel? Employee { get; set; }

    [Required]
    [Column ("ApproveStatus")]
    public CompanyEventProposalStatus ApproveStatus { get; set; }
}
