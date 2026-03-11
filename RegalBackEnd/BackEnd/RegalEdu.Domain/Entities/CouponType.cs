using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("CouponType")]
    public class CouponType : BaseEntity // BaseEntity: Id, CreatedAt, CreatedBy,...
    {
        public string? Name { get; set; } // tên
        public string? Code { get; set; } // Mã loại coupon
        // Hiệu lực
        //public PromotionType? Type { get; set; } = PromotionType.Discount; // Loại coupon (ví dụ: "Chiết khấu")
        public DueType? DueType { get; set; } // Loại thời gian (ví dụ: "Theo thời hạn")
        public int? DurationInDays { get; set; } // Thời hạn sử dụng (nếu có)
        public DateTime? StartDate { get; set; } //ngày bắt đầu 
        public DateTime? EndDate { get; set; }//ngày kết thúc
        // Cấu trúc sinh coupon
        public string? Prefix { get; set; } // Tiền tố
        public string? Suffix { get; set; } // Hậu tố
        public int? CharacterCount { get; set; } // Số lượng ký tự

        // Điều kiện áp dụng
        public bool? ApplyWith { get; set; } // Áp dụng cùng các CTKM khác
        public bool? IsForAllCompany { get; set; } // Áp dụng cho tất cả học viên
        public bool? IsForAllCourse { get; set; } // Áp dụng cho tất cả học viên
        public string? CompanyIds { get; set; } //điều kiện chi nhánh áp dụng
        //[ForeignKey("CompanyId")]
        //public virtual Company? Company { get; set; } //chi nhánh áp dụng
        public string? CourseIds { get; set; } //điều kiện theo khóa học ap dụng
        //[ForeignKey("CourseId")]
        //public virtual Course? Course { get; set; }//khóa học áp dụng

        public int? MinQuantity { get; set; } // Số lượng tối thiểu

        // Học viên áp dụng
        public bool? IsForAllStudents { get; set; } // Áp dụng cho tất cả học viên
        public string? StudentIds { get; set; }

        public string? Note { get; set; } //Ghi chú
        // Mô tả
        public string? Description { get; set; }
        public virtual ICollection<CouponTypeDiscount>? CouponTypeDiscounts { get; set; }
        //public virtual ICollection<CouponTypeFixedPrice>? CouponTypeFixedPrices { get; set; }
        //public virtual ICollection<CouponTypeGift>? CouponTypeGifts { get; set; }
        //public virtual ICollection<CouponTypeCoupon>? CouponTypeCoupons { get; set; } //danh sách học viên được áp dụng
        public virtual ICollection<CouponIssue>? CouponIssues { get; set; }// danh sách coupon được phát hành
                                                                           // public virtual ICollection<CouponStudent>? CouponStudents { get; set; } //danh sách học viên được áp dụng
        public CouponTypeStatus? CouponTypeStatus { get; set; } //trạng thái loại coupon

    }
}
