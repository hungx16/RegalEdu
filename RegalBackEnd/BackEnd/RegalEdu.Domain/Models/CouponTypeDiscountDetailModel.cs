using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponTypeDiscountDetailModel : BaseEntityModel
    {
        public double? MinAmount { get; set; }      //Số tiền chiết khấu tối thiểu là MinAmount|MinQty
        public int Limit { get; set; }// giới hạn số lần sử dụng khuyến mại =0 là không giới hạn
        public int DiscountType { get; set; } // loại chiết khấu là Percentage|FixedAmount
        public double DiscountAmount { get; set; }// giá trị chiết khẩu là Percentage|FixedAmount 
        public Guid? CouponTypeDiscountId { get; set; } //khóa ngoại bảng Discount
        [ForeignKey("CouponTypeDiscountId")]
        public virtual CouponTypeDiscountModel? CouponTypeDiscount { get; set; } //chiết khấu áp dụng
    }

}
