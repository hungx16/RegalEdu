using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    // Bảng chi tiết chương trình khuyến mại: áp dụng giá cố định
    public class CouponTypeFixedPrice : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public double? MinPrice { get; set; } // giá trị đơn hàng tối thiểu để áp dụng giá cố định 
        public int? Limit { get; set; }  // giới hạn số lượng áp dụng giá cố định 0 là không giới hạn
        public double? PriceSale { get; set; } // giá cố định sau khi áp dụng
        public Guid? CouponTypeId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("CouponTypeId")]
        public virtual CouponType? CouponType { get; set; } //chương trình khuyến mại
        

    }
}
