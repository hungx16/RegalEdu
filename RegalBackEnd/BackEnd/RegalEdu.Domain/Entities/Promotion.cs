using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
//chương trinh khuyến mại
public class Promotion : BaseEntity
{
    //public string? Title { get; set; }//tên chương trình 
    public required string Name { get; set; } //tên chương trình
    public string? Description { get; set; }//mô tả
    //public double DiscountRate { get; set; } //không sử dụng
    public DateTime StartDate { get; set; } //ngày bắt đầu 
    public DateTime EndDate { get; set; }//ngày kết thúc
    public bool? ApplyWith { get; set; } //có áp dụng cùng các chương trình khác không
    public Guid? CompanyId { get; set; } //điều kiện chi nhánh áp dụng
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; } //chi nhánh áp dụng
    public Guid? CourseId { get; set; } //điều kiện theo khóa học ap dụng
    [ForeignKey("CourseId")]
    public virtual Course? Course { get; set; }//khóa học áp dụng
    public int Qtymonth { get; set; } //số tháng tối thiểu
    //public Guid? StudentId { get; set; } //điều kiện theo học viên
    //[ForeignKey("StudentId")]
    //public virtual Student? Student { get; set; }
    public bool? CodeUsage { get; set; }// sử dụng mã khuyến mại
    public string? PromoCode { get; set; } //mã khuyến mại
    public int? Type { get; set; } //loại khuyến mại

    public virtual ICollection<Discount>? Discounts { get; set; }
    public virtual ICollection<RegisterStudy>? RegisterStudys { get; set; }
    public virtual ICollection<CourseGift>? CourseGifts { get; set; }
    public virtual ICollection<PromotionCoupon>? PromotionCoupon { get; set; }
    public virtual ICollection<PromotionGift>? PromotionGift { get; set; }
    public virtual ICollection<PromotionFixedPrice>? PromotionFixedPrice { get; set; }
    public bool? AllCompany { get; set; }// sử dụng mã khuyến mại
    public bool? AllCourse { get; set; }// sử dụng mã khuyến mại
    public bool? AllStudent{ get; set; }// sử dụng mã khuyến mại
    public string? Code { get; set; } //Chương trình
    public Guid? PromotionGroupId { get; set; } //Nhóm chương trình
    public virtual PromotionGroup? PromotionGroup { get; set; }
    public virtual ICollection<PromotionStudent>? PromotionStudent { get; set; }
}
