using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Coupon")]
    public class Coupon : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public string? Code { get; set; } // Mã coupon duy nhất (ví dụ: SYSHX123)

        public DateTime CreatedDate { get; set; } // Ngày tạo (thường là IssueDate)
        public DateTime? ExpiredDate { get; set; } // Ngày hết hạn (được tính từ DurationInDays của CouponType)

        //public int? Status { get; set; } // Trạng thái (Ví dụ: "Đã phát hành", "Đã sử dụng", "Đã hết hạn")

        // Người sở hữu/Sử dụng (nếu áp dụng cho học viên cụ thể)
        
        public Guid? RegisterStudyId { get; set; } // Trạng thái (Ví dụ: "Đã phát hành", "Đã sử dụng", "Đã hết hạn")
        public virtual RegisterStudy? RegisterStudy { get; set; }
        public Guid? CouponIssueId { get; set; } // Khóa ngoại liên kết với đợt phát hành
        // Thuộc tính điều hướng
        public virtual CouponIssue? CouponIssue { get; set; } 
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; } // Học viên sở hữu
        public CouponStatus? CouponStatus { get; set; } // Trạng thái (Ví dụ: "Đã phát hành", "Đã sử dụng", "Đã hết hạn")

    }
}
