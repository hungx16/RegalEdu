
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class EnrollmentModel : BaseEntityModel
{
    public Guid StudentId { get; set; }
    public virtual StudentModel? Student { get; set; }
    public Guid ClassId { get; set; }
    public virtual ClassModel? Class { get; set; }
    public Guid? CourseId { get; set; }
    public virtual CourseModel? Course { get; set; }
    public double? Fee { get; set; }
    public double? Discount { get; set; }
    public double? FinalFee { get; set; }
    public PaymentStatus? PaymentCourseStatus { get; set; } //trạng thái thanh toán của khóa học
    public double? PaidAmount { get; set; } //Tổng số tiền đã thanh toán
    public double? UsableAmount { get; set; } //Số tiền còn sử dụng được (nếu có) (tổng số tiền đã đóng - tổng số tiền các buổi học)
    public DateTime? StartDate { get; set; } //ngày bắt đầu học
    public DateTime? EndDate { get; set; } //ngày kết thúc học
    public StudentCourseStatus? StudentCourseStatus { get; set; }
    public int Times { get; set; } //số lần học
    public Guid? ClassTypeId { get; set; } //Loại lớp học
    [ForeignKey("ClassTypeId")]
    public virtual ClassTypeModel? ClassType { get; set; }
    public Guid? RegisterStudyId { get; set; }
    public virtual RegisterStudy? RegisterStudy { get; set; }
}
