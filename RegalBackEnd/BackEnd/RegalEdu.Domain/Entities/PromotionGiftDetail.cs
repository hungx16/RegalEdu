using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Chi tiết quà tặng khuyến mại
    public class PromotionGiftDetail : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public int? GiftName { get; set; } // tên quà tặng
        public int? QuantityGift { get; set; }  //Số lượng quà tặng
        public Guid? PromotionGiftId { get; set; } //khóa ngoại bảng PromotionGift
        [ForeignKey("PromotionGiftId")]
        public virtual PromotionGift? PromotionGift { get; set; } //chương trình khuyến mại

    }
}
