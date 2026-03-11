using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    //bang chi tiết quà tặng/chiết khấu theo khóa học
    public class CourseGiftDeTail : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public Guid? CourseId { get; set; } // khóa ngoại bảng Course
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; } //khóa học
        public double? MinQty { get; set; }     //Số lượng tối thiểu để được quà tặng/chiết khấu là MinAmount|MinQty
        public Guid? CourseGiftId { get; set; }// khóa ngoại bảng CourseGift
        [ForeignKey("CourseGiftId")]
        public virtual CourseGift? CourseGift { get; set; } //quà tặng/chiết khấu cho khóa học
        

    }
}
