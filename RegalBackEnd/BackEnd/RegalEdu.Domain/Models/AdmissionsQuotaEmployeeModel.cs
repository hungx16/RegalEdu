using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;
//ch? ti�u theo chi nh�nh
[Table("AdmissionsQuotaEmployee")]
public class AdmissionsQuotaEmployeeModel : BaseEntityModel
{
    [Required]
    public Guid AdmissionsQuotaId { get; set; } // d? query theo d?t nhanh
    public virtual AdmissionsQuotaDto? AdmissionsQuota { get; set; }
    public Guid? EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    public virtual EmployeeDto? Employee { get; set; }

    public Guid? CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public virtual CompanyDto? Company { get; set; }

    public Guid? RegionId { get; set; }
    [ForeignKey("RegionId")]
    public virtual RegionDto? Region { get; set; }

    [Required]
    public Guid PositionId { get; set; }
    public virtual PositionDto? Position { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RevenuePerSale { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime? AllocationStartAt { get; set; }
    public DateTime? AllocationEndAt { get; set; }
    public int OrderIndex { get; set; }   // NEW

    public Guid? AdmissionsQuotaCompanyId { get; set; }
    public virtual AdmissionsQuotaCompanyDto? AdmissionsQuotaCompany { get; set; }
    public Guid? AdmissionsQuotaRegionId { get; set; }
    public virtual AdmissionsQuotaRegionDto? AdmissionsQuotaRegion { get; set; }
    public QuotaRole QuotaRole { get; set; }   // ASM/BM/SalesLead/Sale/Support/ProbationEmployee/LeavingEmployee




}
