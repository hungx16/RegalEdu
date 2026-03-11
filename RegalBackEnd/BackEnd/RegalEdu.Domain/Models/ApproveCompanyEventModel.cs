using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class ApproveCompanyEventModel : BaseEntityModel
    {
        [Required]
        [Column ("CompanyEventId")]
        public Guid? CompanyEventId { get; set; } = null!;

        [Column ("Reason")]
        public string? Reason { get; set; }
        public Guid? ApprovedBy { get; set; }
        public EmployeeModel? ApprovedUser { get; set; }
        [Required]
        [Column ("ApproveStatus")]
        public CompanyEventProposalStatus ApproveStatus { get; set; }
    }
}
