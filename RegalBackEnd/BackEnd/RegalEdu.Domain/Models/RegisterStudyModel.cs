
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Models;

public class RegisterStudyModel : BaseEntityModel
{
    public int Type { get; set; }
    
    public string? Code { get; set; } //
    public string? CouponCode { get; set; } //Mã giảm
    public string? UsingCouponCode { get; set; } //Mã giảm giá
    public string? CodeParent { get; set; }  //Mã đăng ký học cha
    public Guid? StudentId { get; set; }

    [ForeignKey("StudentId")]
    public StudentModel? Student { get; set; }
    public Guid? CompanyId { get; set; }

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    public Guid? RegionId { get; set; }

    [ForeignKey("RegionId")]
    public virtual RegionModel? Region { get; set; }
    public Guid? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual EmployeeModel? Employee { get; set; }

    public Guid? TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public virtual TeacherModel? Teacher { get; set; }
    public Guid? PromotionId { get; set; }

    [ForeignKey("PromotionId")]
    public virtual PromotionModel? Promotion { get; set; }
    public virtual ICollection<DetailRegisterStudyModel>? DetailRegisterStudys { get; set; }
    public PaymentStatus? PaymentStatus { get; set; } //trạng thái thanh toán
    public double? TotalAmount { get; set; }//tổng tiền

    public virtual ICollection<ReceiptsModel>? Receipts { get; set; }
    public virtual ICollection<RegisterPromotionListModel>? RegisterPromotion { get; set; }
    public virtual ICollection<RegisterGiftModel>? RegisterGifts { get; set; }
    //các trường không có trong entity
    //Các thông tin của học viên đăng ký học
    public string? StudentFullName  { get; set; }
    public string? StudentPhone  { get; set; }
    public string? StudentEmail { get; set; }
    public DateTime? StudentBirthDate { get; set; }//ngày
    public DateTime? ExpectedCompleteDate { get; set; }//ngày sinh                                              //
   
    //các thông tin contact 
    public string? ContactFullName  { get; set; }
    public string? ContactPhone  { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public Relationship ContactRelationship { get; set; } = Relationship.Father;

    //thanh toán
    // Thanh toán (Bước 2)

    public double? TotalDiscount { get; set; }//tổng tiền
    public double? TotalAfterDiscount { get; set; }//tổng tiền
    public double? FirstPaymentAmount { get; set; }//số tiền đã đóng

    public PaymentType? PaymentType { get; set; }//loại thanh toán (Ví dụ: "Một lần | trả góp")
    public PaymenMeThodType? PaymentMethodType { get; set; }//Hình thức thanh toán Thanh toán một lần/ thanh toán nhiều lần
    public PaymenMeThod? PaymentMethod { get; set; }//phương thức thanh toán (Ví dụ: "Tiền mặt | chuyển khoản| vnPay")
                                              //mới bổ sung
    public double? AmountToBePaid { get; set; }//khai báo tổng tiên phải đòng = TotalAmount - TotalDiscount
    public double? TuitionFeesPaid { get; set; }//số tiền đã đóng
    public double? RemainingTuitionFees { get; set; } //số tiền còn lại phải đóng TotalAmount - TuitionFeesPaid

}
