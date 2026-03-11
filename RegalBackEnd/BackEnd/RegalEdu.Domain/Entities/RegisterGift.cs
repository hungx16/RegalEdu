
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class RegisterGift : BaseEntity
{
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }
    public Guid? RegisterStudyId { get; set; }

    [ForeignKey("RegisterStudyId")]
    public virtual RegisterStudy? RegisterStudy { get; set; }
    public Guid? GiftId { get; set; }
    public virtual Gift? Gift { get; set; }
    public string? GiftName { get; set; }
    public int? Amount { get; set; } // số lượng quà
}
