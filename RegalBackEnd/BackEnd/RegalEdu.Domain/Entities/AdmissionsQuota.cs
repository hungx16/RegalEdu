
using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
[Table ("AdmissionsQuota")]
public class AdmissionsQuota : BaseEntity
{
    [Range (2000, 2100)]
    public int Year { get; set; }
    public int? TotalSalesAllocated { get; set; }      // tổng headcount phân bổ (SUM ở Company)
    public int? CurrentSales { get; set; }         // tổng người thực hiện hiện tại (Sales+Lead+Support)

    [Range (1, 12)]
    public int Month { get; set; }


    public QuotaStatus QuotaStage { get; set; } = QuotaStatus.Draft;

    public int CompanyCount { get; set; }


    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalQuota { get; set; } = 0m; // Tổng toàn đợt


    [MaxLength (1000)]
    public string? Note { get; set; }
    public virtual ICollection<AdmissionsQuotaCompany>? AdmissionsQuotaCompanies { get; set; }
    public virtual ICollection<AdmissionsQuotaRegion>? AdmissionsQuotaRegions { get; set; }
    public virtual ICollection<AdmissionsQuotaEmployee>? AdmissionsQuotaEmployees { get; set; }
}
