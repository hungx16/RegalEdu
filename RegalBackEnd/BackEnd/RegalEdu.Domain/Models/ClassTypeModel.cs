using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class ClassTypeModel : BaseEntityModel // Nếu có BaseEntity, sẽ có Id, CreatedAt, CreatedBy...
{
    [Required]
    [MaxLength (50)]
    public string ClassTypeCode { get; set; } = string.Empty; // Mã loại, ví dụ: STANDARD, VIP...

    [Required]
    [MaxLength (200)]
    public string ClassTypeName { get; set; } = string.Empty; // Tên loại, ví dụ: Lớp học tiêu chuẩn

    [MaxLength (1000)]
    public string? Description { get; set; } // Mô tả

    public int MaxStudents { get; set; } // Số học sinh tối đa trong lớp
    public int MinStudents { get; set; } // Số học sinh tối thiểu trong lớp
    public int SessionsPerWeek { get; set; } // Số buổi/tuần
    [Required]

    [Column (TypeName = "decimal(18,2)")]
    public decimal HoursPerSession { get; set; } // Số giờ/buổi

    public ICollection<ClassModel> Classes { get; set; } = new List<ClassModel> ( ); // Danh sách các lớp học thuộc loại này

    public int ClassCount { get; set; } // Số lớp (có thể là số lớp áp dụng cho loại này)
    public virtual ICollection<DetailRegisterStudyModel>? DetailRegisterStudies { get; set; }

}