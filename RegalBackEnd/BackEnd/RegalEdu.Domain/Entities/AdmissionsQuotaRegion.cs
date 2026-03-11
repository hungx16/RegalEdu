using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
//chỉ tiêu theo chi nhánh
[Table ("AdmissionsQuotaRegion")]
public class AdmissionsQuotaRegion : BaseEntity
{
    [Required]
    public Guid AdmissionsQuotaId { get; set; }
    public virtual AdmissionsQuota? AdmissionsQuota { get; set; }
    public int CurrentSales { get; set; } // tổng số lượng sale của vùng
    public int NumberOfSalesAllocated { get; set; }
    [Required]
    public Guid RegionId { get; set; }
    public virtual Region? Region { get; set; }
    public int CompanyCount { get; set; } // Số chi nhánh trong vùng

    [Column (TypeName = "decimal(18,2)")]
    public decimal RevenuePerSale { get; set; } = 0m; // Tổng của vùng trong đợt

    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; } = 0m; // Tổng của vùng trong đợt

    public int OrderIndex { get; set; }   // NEW

    // Navigation
    public virtual ICollection<AdmissionsQuotaCompany> AdmissionsQuotaCompanies { get; set; } = new List<AdmissionsQuotaCompany> ( );

    public virtual ICollection<AdmissionsQuotaAdjustment> AdmissionsQuotaAdjustments { get; set; } = new List<AdmissionsQuotaAdjustment> ( );
    public virtual ICollection<AdmissionsQuotaEmployee> AdmissionsQuotaEmployees { get; set; } = new List<AdmissionsQuotaEmployee> ( );

}
