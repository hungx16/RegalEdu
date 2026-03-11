using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;
[Table ("AdmissionsQuotaCompany")]
//chỉ tiêu theo chi nhánh
public class AdmissionsQuotaCompanyModel : BaseEntityModel
{

    public int? CurrentSales { get; set; } // tổng số lượng sale của công ty

    [Required]
    public Guid AdmissionsQuotaId { get; set; }
    public virtual AdmissionsQuotaDto? AdmissionsQuota { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
    public virtual CompanyDto? Company { get; set; }

    /// <summary>
    /// Số sale được phân bổ ban đầu cho chi nhánh (headcount gốc).
    /// </summary>
    public int NumberOfSalesAllocated { get; set; }
    public int NumberOfPartTimeSales { get; set; }


    /// <summary>
    /// Chỉ tiêu doanh thu trên mỗi EC.
    /// </summary>
    [Column (TypeName = "decimal(18,2)")]
    public decimal RevenuePerSale { get; set; }


    /// <summary>
    /// Tổng chỉ tiêu của chi nhánh (bao gồm cả delta do support nếu có).
    /// </summary>
    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; } = 0m;
    public int OrderIndex { get; set; }   // NEW

    public Guid AdmissionsQuotaRegionId { get; set; }   // <— thêm
    public virtual AdmissionsQuotaRegionDto? AdmissionsQuotaRegion { get; set; } // <— thêm

    public virtual ICollection<AdmissionsQuotaAdjustmentModel> AdmissionsQuotaAdjustments { get; set; } = new List<AdmissionsQuotaAdjustmentModel> ( );
    public virtual ICollection<AdmissionsQuotaEmployeeModel> AdmissionsQuotaEmployees { get; set; } = new List<AdmissionsQuotaEmployeeModel> ( );
}
