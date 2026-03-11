using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("LogRegionCom")]
    public class LogRegionCom : BaseEntity
    {
        /// <summary>
        /// FK: Chi nhánh
        /// </summary>

        public Guid? CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public Company? Company { get; set; } = null!;

        /// <summary>
        /// FK: Vùng được phân bổ
        /// </summary>
        [Required]
        public Guid RegionId { get; set; }

        [ForeignKey ("RegionId")]
        public Region? Region { get; set; } = null!;

        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }
    }
}
