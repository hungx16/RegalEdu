using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho Bảng quà tặng/chiết khấu cho khóa học
    public class CourseGiftModel : BaseEntityModel
    {
        public bool? AllCourse { get; set; }
        public double? MinQty { get; set; }
        public Guid? PromotionId { get; set; }
        public virtual ICollection<CourseGiftDeTailModel>? CourseGiftDeTails { get; set; }
    }

}
