using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class LogEmployeePositionDto : BaseEntityModel
    {

        public Guid? EmployeeId { get; set; }

        [ForeignKey ("EmployeeId")]
        public EmployeeDto? Employee { get; set; } = null!;

        public Guid CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public CompanyDto? Company { get; set; } = null!;

        [Required]
        public Guid PositionId { get; set; }

        [ForeignKey ("PositionId")]
        public PositionDto? Position { get; set; } = null!;
        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }
    }
}
