using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class RegisterGiftModel : BaseEntityModel
{
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }
    public Guid? RegisterStudyId { get; set; }

    [ForeignKey("RegisterStudyId")]
    public virtual RegisterStudy? RegisterStudy { get; set; }
    public Guid? GiftId { get; set; }
    public virtual GiftModel? Gift { get; set; }
    public string? GiftName { get; set; }
    public int? Amount { get; set; } // số lượng quà
}
