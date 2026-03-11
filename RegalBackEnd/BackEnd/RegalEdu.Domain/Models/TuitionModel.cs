using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class TuitionModel : BaseEntityModel
{
    public string TuitionCode { get; set; } = string.Empty;

    [Required]
    [StringLength (255)]
    public string TuitionName { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    public virtual CourseModel? Course { get; set; } // 1 Tuition thuộc về 1 Course

    public Guid ClassTypeId { get; set; }
    public virtual ClassTypeModel? ClassType { get; set; } // 1 Tuition thuộc về 1 ClassType

    // Số giờ học (DurationHours <= 1000)
    [Required]
    [Range (1, 1000)]
    [Column (TypeName = "decimal(18,2)")]

    public decimal DurationHours { get; set; }

    // Số giờ đăng ký tối thiểu (0 < MinHours ≤ DurationHours)
    [Required]
    [Range (1, 1000)]
    [Column (TypeName = "decimal(18,2)")]

    public decimal MinHours { get; set; }

    // Số tháng học (TotalMonths ≤ 100)       
    [Column (TypeName = "decimal(18,2)")]

    public decimal TotalMonths { get; set; }

    // Đơn vị tính (Hour/Session/Month/Course)
    [Required]
    public UnitType? Unit { get; set; }

    // Học phí
    [Column (TypeName = "decimal(18,2)")]
    public decimal? TuitionFee { get; set; }
    public List<CourseLessonModel>? CourseLessons { get; set; } = new List<CourseLessonModel> ( );

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
