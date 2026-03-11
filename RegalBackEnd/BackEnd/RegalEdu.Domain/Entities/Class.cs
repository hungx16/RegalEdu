using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
[Table ("Class")]
public class Class : BaseEntity
{
    public string ClassCode { get; set; } = string.Empty; // Mã lớp học sinh theo qui tắc
    [Required]
    [StringLength (255)]
    public required string ClassName { get; set; }
    [Required]
    public Guid CourseId { get; set; } // FK -> Course đang hoạt động
    [ForeignKey (nameof (CourseId))]
    public virtual Course? Course { get; set; }
    [Required]
    public Guid CompanyId { get; set; } // FK -> Company đang hoạt động
    [ForeignKey (nameof (CompanyId))]
    public virtual Company? Company { get; set; }
    public ClassMethod Method { get; set; } = ClassMethod.Onsite;//// 0 = Học trực tiếp, 1 = Học online
    public DateTime StartDate { get; set; } = DateTime.Now;//Mặc định là ngày hiện tại
    public string? Description { get; set; } // Mô tả lớp học
    public bool TrialClass { get; set; } = false; // true = thử; false = không thử
    public ClassStatus ClassStatus { get; set; } = ClassStatus.Plan;//Mặc định trạng thái là Kế hoạch
    public Guid ClassTypeId { get; set; } // Loại chương trình (FK -> Tuition)
    [ForeignKey (nameof (ClassTypeId))]
    public virtual ClassType? ClassType { get; set; }
    public Guid? TeacherId { get; set; } // Giáo viên
    [ForeignKey (nameof (TeacherId))]
    public virtual Teacher? Teacher { get; set; }// Có status = 1
    // ClassSchedule lưu các thứ và ca học trong tuần
    // ClassSchedule sẽ lưu các id của WorkingTime vào csdl ngăn cách nhau bởi #$#. Ví dụ “id1#$#id2”
    public String? ClassSchedule { get; set; } = string.Empty;
    public Guid? EmployeeId { get; set; } // Nhân viên phụ trách lớp có status = 1
    [ForeignKey (nameof (EmployeeId))]
    public virtual Employee? Employee { get; set; }
    public DateTime? EndDate { get; set; }//Hệ thống tự động tính

    public virtual ICollection<Enrollment>? Enrollments { get; set; }
    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; }
    //Hải bổ sung 06-01
    public Guid? ClassSeriesId { get; set; } // null = lớp đơn, có giá trị = thuộc chuỗi

}
