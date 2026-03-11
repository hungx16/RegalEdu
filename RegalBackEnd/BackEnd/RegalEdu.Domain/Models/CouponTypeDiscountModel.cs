using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponTypeDiscountModel : BaseEntityModel
    {
        public int? Method { get; set; } // phương thức chiết khấu 
        public double? DiscountMax { get; set; }      //Số tiền chiết khấu tối đa 
        public Guid? CouponTypeId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("CouponTypeId")]
        public virtual CouponType? CouponTypes { get; set; } //chương trình khuyến
        public virtual ICollection<CouponTypeDiscountDetailModel>? CouponTypeDiscountDetail { get; set; }
    }

}
