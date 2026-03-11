using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class DetailRegisterStudyModel : BaseEntityModel
{
    // Foreign Key properties
    public Guid? RegisterStudyId { get; set; }
    public Guid? CourseId { get; set; }

    public UnitType? Unit { get; set; } // e.g., number of units for the course in this registration
    public double? TuitionFee { get; set; } // e.g., giá tiền cho khóa học trong đăng ký này
    public double? DiscountAmount { get; set; } // e.g., số tiền giảm giá cho khóa học trong đăng ký này
    public double? PaidAmount { get; set; } // e.g., số tiền đã thanh toán cho khóa học trong đăng ký này
    public double? TotalAmount { get; set; } // e.g., tổng số tiền sau giảm giá cho khóa học trong đăng ký này
    // Navigation properties for foreign keys
    [ForeignKey("RegisterStudyId")]
    public virtual RegisterStudy? RegisterStudy { get; set; }

    [ForeignKey("CourseId")]
    public virtual CourseModel? Course { get; set; }
    public Guid? ClassTypeId { get; set; }
    [ForeignKey("ClassTypeId")]
    public virtual ClassTypeModel? ClassType { get; set; }
    public int Quantity { get; set; }
}
