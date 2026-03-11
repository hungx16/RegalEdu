using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("LogRegionCom")]
    public class LogRegionComDto : BaseEntityModel
    {
        public new Guid Id { get; set; }
        /// <summary>
        /// FK: Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public CompanyDto? Company { get; set; } = null!;

        /// <summary>
        /// FK: Vùng được phân bổ
        /// </summary>
        [Required]
        public Guid RegionId { get; set; }

        [ForeignKey ("RegionId")]
        public RegionDto? Region { get; set; } = null!;

        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }
    }
}
