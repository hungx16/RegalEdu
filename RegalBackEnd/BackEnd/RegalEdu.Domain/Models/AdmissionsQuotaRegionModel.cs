using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;
//chỉ tiêu theo chi nhánh
[Table ("AdmissionsQuotaRegion")]
public class AdmissionsQuotaRegionModel : BaseEntityModel
{
    [Required]
    public Guid AdmissionsQuotaId { get; set; }
    public virtual AdmissionsQuotaDto? AdmissionsQuota { get; set; }
    public int CurrentSales { get; set; } // tổng số lượng sale của vùng
    public int NumberOfSalesAllocated { get; set; }

    [Required]
    public Guid RegionId { get; set; }
    public virtual RegionDto? Region { get; set; }

    public int CompanyCount { get; set; } // Số chi nhánh trong vùng

    [Column (TypeName = "decimal(18,2)")]
    public decimal RevenuePerSale { get; set; } = 0m; // Tổng của vùng trong đợt

    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; } = 0m; // Tổng của vùng trong đợt
    public int OrderIndex { get; set; }   // NEW


    // Navigation
    public virtual ICollection<AdmissionsQuotaCompanyModel> AdmissionsQuotaCompanies { get; set; } = new List<AdmissionsQuotaCompanyModel> ( );

    public virtual ICollection<AdmissionsQuotaAdjustmentModel> AdmissionsQuotaAdjustments { get; set; } = new List<AdmissionsQuotaAdjustmentModel> ( );
    public virtual ICollection<AdmissionsQuotaEmployeeModel> AdmissionsQuotaEmployees { get; set; } = new List<AdmissionsQuotaEmployeeModel> ( );
}
