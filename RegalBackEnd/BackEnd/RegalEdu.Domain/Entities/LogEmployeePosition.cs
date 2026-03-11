using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("LogEmployeePosition")]
    public class LogEmployeePosition : BaseEntity
    {

        public Guid? EmployeeId { get; set; }

        [ForeignKey ("EmployeeId")]
        public Employee? Employee { get; set; } = null!;
        [Required]
        public Guid? CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public Company? Company { get; set; } = null!;

        [Required]
        public Guid PositionId { get; set; }

        [ForeignKey ("PositionId")]
        public Position? Position { get; set; } = null!;

        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
