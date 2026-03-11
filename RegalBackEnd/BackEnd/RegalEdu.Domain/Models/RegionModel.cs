using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class RegionModel : BaseEntityModel
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
        public ICollection<CompanyDto> Companies { get; set; } = new List<CompanyDto> ( );

        /// <summary>
        /// FK đến người quản lý (employee)
        /// </summary>
        public Guid? ManagerId { get; set; }

        [ForeignKey ("ManagerId")]
        public EmployeeDto? Manager { get; set; }
        public virtual ICollection<RegisterStudyModel>? RegisterStudies { get; set; }
        public virtual ICollection<AdmissionsQuotaRegionModel>? AdmissionsQuotaRegions { get; set; }
        //Hải bổ sung 2609
        // 🔹 Quan hệ: Một vùng có nhiều AllocationDetailEvent
        public List<AllocationDetailEventModel>? AllocationDetailEvents { get; set; } = new List<AllocationDetailEventModel>();
    }
}
