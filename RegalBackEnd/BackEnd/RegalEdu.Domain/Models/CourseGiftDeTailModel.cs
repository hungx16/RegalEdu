using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho bảng chi tiết quà tặng/chiết khấu theo khóa học
    public class CourseGiftDeTailModel : BaseEntityModel
    {
        public Guid? CourseId { get; set; }
        public double? MinQty { get; set; }
        public Guid? CourseGiftId { get; set; }
    }

}
