using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho bảng phiếu quà tặng
    public class PromotionStudentModel : BaseEntityModel
    {
        public Guid? StudentId { get; set; } // phương thức quà tặng
        public virtual StudentModel? Student { get; set; } //chương trình khuyến mại
        public Guid? PromotionId { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("PromotionId")]
        public virtual PromotionModel? Promotion { get; set; } //chương trình khuyến mại
    }

}
