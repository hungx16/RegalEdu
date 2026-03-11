using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Bảng quà tặng/chiết khấu cho khóa học
    public class CourseGift : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public bool? AllCourse { get; set; } //Áp dụng cho tất cả khóa học
        public double? MinQty { get; set; }     //Số lượng tối thiểu để được quà tặng/chiết khấu là MinAmount|MinQty
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; } //chương trình khuyến mại
        public virtual ICollection<CourseGiftDeTail>? CourseGiftDeTails { get; set; }

    }
}
