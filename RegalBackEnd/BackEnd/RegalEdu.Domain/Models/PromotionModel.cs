using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class PromotionModel : BaseEntityModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? ApplyWith { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CourseId { get; set; }
        public int? Qtymonth { get; set; }
        public Guid? StudentId { get; set; }
        public bool? CodeUsage { get; set; }
        public string? PromoCode { get; set; }
        public int? Type { get; set; }

        // Các thuộc tính đã được cập nhật
        public ICollection<DiscountModel>? Discounts { get; set; }
        public ICollection<RegisterStudyModel>? RegisterStudys { get; set; }
        public ICollection<CourseGiftModel>? CourseGifts { get; set; }
        public ICollection<PromotionCouponModel>? PromotionCoupon { get; set; }
        public ICollection<PromotionGiftModel>? PromotionGift { get; set; }
        public ICollection<PromotionFixedPriceModel>? PromotionFixedPrice { get; set; }
        public bool? AllCompany { get; set; }// sử dụng mã khuyến mại
        public bool? AllCourse { get; set; }// sử dụng mã khuyến mại
        public bool? AllStudent { get; set; }// sử dụng mã khuyến mại
        public string? Code { get; set; } //Chương trình
        public Guid? PromotionGroupId { get; set; } //Nhóm chương trình
        public virtual PromotionGroup? PromotionGroup { get; set; }
        public ICollection<PromotionStudentModel>? PromotionStudent { get; set; }
    }
}
