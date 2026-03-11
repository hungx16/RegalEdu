using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponIssueModel : BaseEntityModel
    {
        public Guid? CouponTypeId { get; set; } // Khóa ngoại liên kết với Loại Coupontype

        public IssueType? IssueType { get; set; } // Loại phát hành (ví dụ: "theo số lượng| theo học viên được chọn")
        public int? Quantity { get; set; } // Số lượng phát hành (*)
        public DateTime? IssueDate { get; set; } // Ngày phát hành (*)
        public IssueStatus IssueStatus { get; set; }
        // Học viên áp dụng (từ hình ảnh thứ hai)
        public bool? IsForAllStudents { get; set; } // Áp dụng cho tất cả học viên

        public virtual CouponTypeModel? CouponType { get; set; }
        public virtual ICollection<CouponModel>? Coupons { get; set; } // Danh sách các mã coupon cụ thể được phát hành trong đợt này
        public virtual ICollection<CouponIssueStudentModel>? CouponIssueStudent { get; set; } //danh sách học viên được áp dụng

    }

}
