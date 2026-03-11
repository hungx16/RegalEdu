using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho Quà tặng của chương trình khuyến mại
    public class PromotionGiftModel : BaseEntityModel
    {
        public int? GiftType { get; set; }
        public int? GiftCount { get; set; }
        public Guid? PromotionId { get; set; }
        public virtual ICollection<PromotionGiftDetailModel>? PromotionGiftDetails { get; set; }
    }

}
