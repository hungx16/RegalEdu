
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class RegisterStudy : BaseEntity
{
    
    public int Type { get; set; } //Đăng ký mới | Đăng ký lại
    public string? Code { get; set; } //
    public string? CouponCode { get; set; } //Mã giảm giá
    public string? UsingCouponCode { get; set; } //Mã giảm giá
    public string? CodeParent { get; set; }  //Mã đăng ký học cha
    public Guid? StudentId { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }
    public Guid? CompanyId { get; set; } 

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    public Guid? RegionId { get; set; }

    [ForeignKey("RegionId")]
    public virtual Region? Region { get; set; }
    public Guid? EmployeeId { get; set; } 

    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }//nhân viên tư vấn

    public Guid? TeacherId { get; set; }//giáo viên phụ trách

    [ForeignKey("TeacherId")]
    public virtual Teacher? Teacher { get; set; } //
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }
    public virtual ICollection<DetailRegisterStudy>? DetailRegisterStudys { get; set; }
    public PaymentStatus? PaymentStatus { get; set; } //trạng thái thanh toán 0-chưa thanh toán, 1-Thanh toán 1 phần, 2-đã thanh toán
    public double? TotalAmount { get; set; }//tổng tiền
    
    public virtual ICollection<Receipts>? Receipts { get; set; }
    public virtual ICollection<RegisterPromotionList>? RegisterPromotionList { get; set; }
    public double? TotalDiscount { get; set; }//tổng tiền khuyến mại
    //khai báo tổng tiên phải đòng = TotalAmount - TotalDiscount
    public double? AmountToBePaid { get; set; }
    public double? TuitionFeesPaid { get; set; }//số tiền đã đóng
    public double? RemainingTuitionFees { get; set; } //số tiền còn lại phải đóng TotalAmount - TuitionFeesPaid
    public virtual ICollection<RegisterGift>? RegisterGifts { get; set; }

}
