using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;
//chỉ tiêu theo chi nhánh
[Table ("AdmissionsQuotaAdjustment")]
public class AdmissionsQuotaAdjustmentModel : BaseEntityModel
{
    /// <summary>
    /// Phạm vi điều chỉnh: Region / Company
    /// </summary>
    public AdjustmentScope Scope { get; set; }

    // Chỉ chọn 1: vùng hoặc chi nhánh (snapshot theo đợt)
    public Guid? AdmissionsQuotaRegionId { get; set; }
    public virtual AdmissionsQuotaRegionModel? AdmissionsQuotaRegion { get; set; }

    public Guid? AdmissionsQuotaCompanyId { get; set; }
    public virtual AdmissionsQuotaCompanyModel? AdmissionsQuotaCompany { get; set; }
    [Column (TypeName = "decimal(18,2)")]

    public decimal? TotalQuotaBefore { get; set; } //Tổng doanh thu trước điều chỉnh
    [Column (TypeName = "decimal(18,2)")]

    public decimal? TotalQuotaAfter { get; set; } //Tổng doanh thu sau điều chỉnh

    [MaxLength (1000)]
    public string? Reason { get; set; }

}
