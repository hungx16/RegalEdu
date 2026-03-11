using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("ApproveCompanyEvent")]
    public class ApproveCompanyEvent : BaseEntity
    {
        [Required]
        [Column ("CompanyEventId")]
        public Guid? CompanyEventId { get; set; }

        [Column ("Reason")]
        public string? Reason { get; set; }
        [ForeignKey ("EmployeeId")]
        public Guid? ApprovedBy { get; set; }
        public Employee? ApprovedUser { get; set; }

        [Required]
        [Column ("ApproveStatus")]
        public CompanyEventProposalStatus ApproveStatus { get; set; }
    }
}
