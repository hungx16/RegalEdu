using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("LogEmployeePosition")]
    public class LogEmployeePositionModel : BaseEntityModel
    {

        public Guid? EmployeeId { get; set; }

        [ForeignKey ("EmployeeId")]
        public EmployeeModel? Employee { get; set; } = null!;
        [Required]
        public Guid? CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public CompanyModel? Company { get; set; } = null!;


        [Required]
        public Guid PositionId { get; set; }

        [ForeignKey ("PositionId")]
        public PositionModel? Position { get; set; } = null!;

        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
