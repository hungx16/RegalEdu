
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class RegisterPromotionList : BaseEntity
{
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }
    public Guid? RegisterStudyId { get; set; }

    [ForeignKey("RegisterStudyId")]
    public virtual RegisterStudy? RegisterStudy { get; set; }
    public double? DiscountAmount { get; set; }
}
