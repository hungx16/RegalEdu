using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponTypeGiftModel : BaseEntityModel
    {
        public int? GiftType { get; set; } // phương thức quà tặng
        public int? GiftCount { get; set; }  //Số lượng quà tặng
        public Guid? CouponTypeId { get; set; } //khóa ngoại bảng loại coupon
        [ForeignKey("CouponTypeId")]
        public virtual CouponTypeModel? CouponType { get; set; } //chương trình khuyến mại
        public virtual ICollection<CouponTypeGiftDetailModel>? CouponTypeGiftDetail { get; set; }

    }

}
