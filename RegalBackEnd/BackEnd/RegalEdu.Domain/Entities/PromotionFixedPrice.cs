using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    // Bảng chi tiết chương trình khuyến mại: áp dụng giá cố định
    public class PromotionFixedPrice : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public double? MinPrice { get; set; } // phương thức quà tặng
        public int? Limit { get; set; }  // giới hạn số lượng áp dụng giá cố định 0 là không giới hạn
        public double? PriceSale { get; set; } //khóa ngoại bảng Promotion
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; } //chương trình khuyến mại
        

    }
}
