using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Quà tặng của chương trình khuyến mại
    public class PromotionGift : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public int? GiftType { get; set; } // phương thức quà tặng
        public int? GiftCount { get; set; }  //Số lượng quà tặng
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; } //chương trình khuyến mại
        public virtual ICollection<PromotionGiftDetail>? PromotionGiftDetails { get; set; }

    }
}
