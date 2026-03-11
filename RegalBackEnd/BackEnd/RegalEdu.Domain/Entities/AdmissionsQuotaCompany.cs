using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
[Table ("AdmissionsQuotaCompany")]
//chỉ tiêu theo chi nhánh
public class AdmissionsQuotaCompany : BaseEntity
{

    public int? CurrentSales { get; set; } //số người thực hiện hiện tại = Sales + SalesLead + Support (active trong tháng).

    [Required]
    public Guid AdmissionsQuotaId { get; set; }
    public virtual AdmissionsQuota? AdmissionsQuota { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }

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
    public int OrderIndex { get; set; }   // NEW


    /// <summary>
    /// Tổng chỉ tiêu của chi nhánh (bao gồm cả delta do support nếu có).
    /// </summary>
    [Column (TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; } = 0m;

    public Guid AdmissionsQuotaRegionId { get; set; }   // <— thêm
    public virtual AdmissionsQuotaRegion? AdmissionsQuotaRegion { get; set; } // <— thêm

    public virtual ICollection<AdmissionsQuotaAdjustment> AdmissionsQuotaAdjustments { get; set; } = new List<AdmissionsQuotaAdjustment> ( );
    public virtual ICollection<AdmissionsQuotaEmployee> AdmissionsQuotaEmployees { get; set; } = new List<AdmissionsQuotaEmployee> ( );


}
