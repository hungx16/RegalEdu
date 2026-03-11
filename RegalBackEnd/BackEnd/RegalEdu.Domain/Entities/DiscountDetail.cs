using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    // Chi tiết chiết khấu
    public class DiscountDetail : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
       
        public double? MinAmount { get; set; }      //Số tiền chiết khấu tối thiểu là MinAmount|MinQty
        public int Limit { get; set; }// giới hạn số lần sử dụng khuyến mại =0 là không giới hạn
        public int DiscountType { get; set; } // loại chiết khấu là Percentage|FixedAmount
        public double DiscountAmount { get; set; }// giá trị chiết khẩu là Percentage|FixedAmount 
        public Guid? DiscountId { get; set; } //khóa ngoại bảng Discount
        [ForeignKey("DiscountId")]
        public virtual Discount? Discount { get; set; } //chiết khấu áp dụng
    }
}
