using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponStudentModel : BaseEntityModel
    {
        public Guid? CouponTypeId { get; set; } // Khóa ngoại liên kết với Loại Coupontype
        [ForeignKey("CouponTypeId")]
        public virtual CouponTypeModel? CouponType { get; set; } //loại coupon
        public Guid? StudentId { get; set; } //điều kiện theo học viên
        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }
        public string? StudentName { get; set; }
        
    }

}
