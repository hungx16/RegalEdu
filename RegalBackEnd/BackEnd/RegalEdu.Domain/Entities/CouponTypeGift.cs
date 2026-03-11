using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Quà tặng của chương trình khuyến mại
    public class CouponTypeGift : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public int? GiftType { get; set; } // phương thức quà tặng
        public int? GiftCount { get; set; }  //Số lượng quà tặng
        public Guid? CouponTypeId { get; set; } //khóa ngoại bảng loại coupon
        [ForeignKey("CouponTypeId")]
        public virtual CouponType? CouponType { get; set; } //chương trình khuyến mại
        public virtual ICollection<CouponTypeGiftDetail>? CouponTypeGiftDetail { get; set; }

    }
}
