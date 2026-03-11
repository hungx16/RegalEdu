using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //bảng chiết khấu dùng cho các chương trình khuyến mại
    public class CouponTypeDiscount : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public int? Method { get; set; } // phương thức chiết khấu 
        public double? DiscountMax { get; set; }      //Số tiền chiết khấu tối đa 
        public Guid? CouponTypeId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("CouponTypeId")]
        public virtual CouponType? CouponTypes { get; set; } //chương trình khuyến
        public virtual ICollection<CouponTypeDiscountDetail>? CouponTypeDiscountDetail { get; set; }


    }
}
