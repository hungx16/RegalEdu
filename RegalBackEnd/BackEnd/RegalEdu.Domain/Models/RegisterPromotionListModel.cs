using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class RegisterPromotionListModel : BaseEntityModel
{
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }
    public Guid? RegisterStudyId { get; set; }

    [ForeignKey("RegisterStudyId")]
    public virtual RegisterStudy? RegisterStudy { get; set; }
    public double? DiscountAmount { get; set; }
}
