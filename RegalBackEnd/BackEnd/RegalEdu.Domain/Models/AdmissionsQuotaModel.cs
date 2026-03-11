
using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;
[Table ("AdmissionsQuota")]
public class AdmissionsQuotaModel : BaseEntityModel
{
    [Range (2000, 2100)]
    public int Year { get; set; }


    [Range (1, 12)]
    public int Month { get; set; }

    public int CurrentSales { get; set; } // tổng số lượng sale của vùng
    public int TotalSalesAllocated { get; set; }
    public QuotaStatus QuotaStage { get; set; } = QuotaStatus.Draft;

    public int CompanyCount { get; set; }

    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalQuota { get; set; } = 0m; // Tổng toàn đợt


    [MaxLength (1000)]
    public string? Note { get; set; }
    public virtual ICollection<AdmissionsQuotaCompanyModel>? AdmissionsQuotaCompanies { get; set; }
    public virtual ICollection<AdmissionsQuotaRegionModel>? AdmissionsQuotaRegions { get; set; }
    public virtual ICollection<AdmissionsQuotaEmployeeModel>? AdmissionsQuotaEmployees { get; set; }
}
