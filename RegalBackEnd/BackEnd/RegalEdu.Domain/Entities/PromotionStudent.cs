using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //Bảng phiếu quà tặng
    public class PromotionStudent : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public Guid? StudentId { get; set; } // phương thức quà tặng
        public virtual Student? Student { get; set; } //chương trình khuyến mại
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; } //chương trình khuyến mại

    }
}
