using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("CouponStudent")]//bảng liên kết nhiều nhiều giữa coupontype và học viên để xác định học viên nào được áp dụng loại coupon nào
    public class CouponStudent : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public Guid? CouponTypeId { get; set; } // Khóa ngoại liên kết với Loại Coupontype
        [ForeignKey("CouponTypeId")]
        public virtual CouponType? CouponType { get; set; } //loại coupon
        public Guid? StudentId { get; set; } //điều kiện theo học viên
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        public string? StudentName { get; set; }

    }
}
