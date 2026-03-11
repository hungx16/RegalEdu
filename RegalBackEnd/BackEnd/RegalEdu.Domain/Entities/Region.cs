using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Region")]
    public class Region : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string RegionCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string RegionName { get; set; } = string.Empty;

        [MaxLength (1000)]
        public string? Description { get; set; }

        // Quan hệ: Một vùng có nhiều chi nhánh
        public ICollection<Company> Companies { get; set; } = new List<Company> ( );

        /// <summary>
        /// FK đến người quản lý (employee)
        /// </summary>
        public Guid? ManagerId { get; set; }

        [ForeignKey ("ManagerId")]
        public Employee? Manager { get; set; }
        public virtual ICollection<RegisterStudy>? RegisterStudies { get; set; }
        public virtual ICollection<AdmissionsQuotaRegion>? AdmissionsQuotaRegions { get; set; }

        //Hải bổ sung 2609
        // 🔹 Quan hệ: Một vùng có nhiều AllocationDetailEvent
        public virtual ICollection<AllocationDetailEvent>? AllocationDetailEvents { get; set; } = new List<AllocationDetailEvent>();
    }
}
