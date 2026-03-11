using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //bảng chiết khấu dùng cho các chương trình khuyến mại
    public class Discount : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public int? Method { get; set; } // phương thức chiết khấu 
        public double? DiscountMax { get; set; }      //Số tiền chiết khấu tối đa 
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; } //chương trình khuyến
        
        public virtual ICollection<DiscountDetail>? DiscountDetails { get; set; }


    }
}
