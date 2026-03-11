using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponTypeGiftDetailModel : BaseEntityModel
    {
        public int? GiftName { get; set; } // tên quà tặng
        public int? QuantityGift { get; set; }  //Số lượng quà tặng
        public Guid? CouponTypeGiftId { get; set; } //khóa ngoại bảng PromotionGift
        [ForeignKey("CouponTypeGiftId")]
        public virtual CouponTypeGiftModel? CouponTypeGift { get; set; } //chương trình khuyến mại

    }

}
