using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("CouponIssue")]
    public class CouponIssue : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        // Thông tin phát hành (Batch/Đợt phát hành)
      //  public int Id { get; set; } // Khóa chính
        public Guid? CouponTypeId { get; set; } // Khóa ngoại liên kết với Loại Coupontype

        public IssueType? IssueType { get; set; } // Loại phát hành (ví dụ: "Số lượng phát hành")
        public int? Quantity { get; set; } // Số lượng phát hành (*)
        public DateTime? IssueDate { get; set; } // Ngày phát hành (*)

        // Học viên áp dụng (từ hình ảnh thứ hai)
        public bool? IsForAllStudents { get; set; } // Áp dụng cho tất cả học viên
        public IssueStatus IssueStatus { get; set; }

        // Thuộc tính điều hướng
        public virtual CouponType? CouponType { get; set; }
        public virtual ICollection<Coupon>? Coupons { get; set; } // Danh sách các mã coupon cụ thể được phát hành trong đợt này
        public virtual ICollection<CouponIssueStudent>? CouponIssueStudent { get; set; } //danh sách học viên được áp dụng

    }
}
