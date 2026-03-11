
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class Receipts : BaseEntity
{
   
    public ReceiptType? ReceiptType { get; set; } // Loại phiếu thu (Ví dụ: "Phiếu theo đơn")
    public string? ReceiptCode { get; set; } // Mã phiếu thu


    // Khóa ngoại liên kết
    public Guid? RegisterStudyId { get; set; } // Mã đăng ký học
    public Guid? StudentId { get; set; } // Học viên
    public Guid? CourseId { get; set; } // Chương trình/Khóa học (Có thể là khóa học chính)
    public Guid? EmployeeId { get; set; } // Nhân viên tư vấn

    // Chi tiết thanh toán
    public PaymentType? PaymentType { get; set; } // Thanh toán một lần / Trả góp
    public PaymenMeThodType? PaymentMethodType { get; set; }//Hình thức thanh toán Thanh toán một lần/ thanh toán nhiều lần
    public PaymenMeThod? PaymentMethod { get; set; } // Phương thức thanh toán (Ví dụ: "Chuyển khoản")
    
    public double? TotalAmount { get; set; } // Tổng tiền thanh toán

    // Ghi chú
    public string? Note { get; set; }

    // Thuộc tính điều hướng
    public virtual Student? Student { get; set; }
    public virtual Course? Course { get; set; }
    public virtual Employee? Employee { get; set; }
    public virtual RegisterStudy? RegisterStudy { get; set; }

    //public Guid? RegionId { get; set; } // Vùng
    //public virtual Region? Region { get; set; }
    //public Guid? CompanyId { get; set; } // Vùng
    //public virtual Company? Company { get; set; }
}
