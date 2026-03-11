using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Bảng phiếu quà tặng
    public class CouponTypeCoupon : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public double? MinQuantity { get; set; } // phương thức quà tặng
        public int? Limit { get; set; }  //Số lượng quà tặng
        public string? CouponCode { get; set; } //khóa ngoại bảng Promotion
        public Guid? CouponTypeID { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("CouponTypeID")]
        public virtual CouponType? CouponType { get; set; } //chương trình khuyến mại
        //public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        //[ForeignKey("PromotionId")]
        //public virtual Promotion? Promotion { get; set; } //chương trình khuyến mại
        

    }
}
