
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid? StudentId { get; set; }
    public virtual Student? Student { get; set; }
    public Guid? ClassId { get; set; }
    public virtual Class? Class { get; set; }
    public Guid? CourseId { get; set; }
    public virtual Course? Course { get; set; }
    public double? Fee { get; set; } //học phí
    public double? Discount { get; set; } //khuyến mại giám giá
    public double? FinalFee { get; set; }// giá trị cuôi TuitionAfterDiscount (học phí - giảm giá)
    public PaymentStatus? PaymentCourseStatus { get; set; } //trạng thái thanh toán của khóa học
    public double? PaidAmount { get; set; } //Tổng số tiền đã thanh toán - Đang giống FinalFee?
    public double? UsableAmount { get; set; } //Số tiền còn sử dụng được (nếu có) (tổng số tiền đã đóng - tổng số tiền các buổi học)
    public DateTime? StartDate { get; set; } //ngày bắt đầu học
    public DateTime? EndDate { get; set; } //ngày hết tiền học
    public StudentCourseStatus? StudentCourseStatus { get; set; }// 0 - Chưa học, 1 - Đang học, 2 - Hoàn thành, 3 - Bảo lưu, 4 - Hủy    
    public int Times { get; set; } //số lần học
    public Guid? ClassTypeId { get; set; } //Loại lớp học
    [ForeignKey("ClassTypeId")]
    public virtual ClassType? ClassType { get; set; }
    public Guid? RegisterStudyId { get; set; }
    public virtual RegisterStudy? RegisterStudy { get; set; }
}
