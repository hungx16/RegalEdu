using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
//ch? tiêu theo chi nhánh
[Table ("AdmissionsQuotaEmployee")]
public class AdmissionsQuotaEmployee : BaseEntity
{
    [Required]
    public Guid AdmissionsQuotaId { get; set; } // d? query theo d?t nhanh
    public virtual AdmissionsQuota? AdmissionsQuota { get; set; }
    public Guid? EmployeeId { get; set; }
    [ForeignKey ("EmployeeId")]
    public virtual Employee? Employee { get; set; }
    // NEW: xác d?nh d?i tu?ng nhân viên trong d?t
    public Guid? CompanyId { get; set; }
    [ForeignKey ("CompanyId")]
    public virtual Company? Company { get; set; }

    public Guid? RegionId { get; set; }
    [ForeignKey ("RegionId")]
    public virtual Region? Region { get; set; }

    public QuotaRole QuotaRole { get; set; }   // ASM/BM/SalesLead/Sale/Support/ProbationEmployee/LeavingEmployee


    [Required]
    public Guid PositionId { get; set; }
    public virtual Position? Position { get; set; }

    [Column (TypeName = "decimal(18,2)")]
    public decimal? RevenuePerSale { get; set; } //ch? tiêu doanh thu c?a sale
    public DateTime? JoinedAt { get; set; }
    public DateTime? AllocationStartAt { get; set; }
    public DateTime? AllocationEndAt { get; set; }
    public Guid? AdmissionsQuotaCompanyId { get; set; }
    public int OrderIndex { get; set; }   // NEW

    public virtual AdmissionsQuotaCompany? AdmissionsQuotaCompany { get; set; }
    public Guid? AdmissionsQuotaRegionId { get; set; }
    public virtual AdmissionsQuotaRegion? AdmissionsQuotaRegion { get; set; }
}
