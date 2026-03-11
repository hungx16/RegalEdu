using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("CourseLesson")]
public class CourseLesson : BaseEntity
{
    // Khóa ngoại đến bảng Course (chi tiết khóa học)
    [Required]
    public Guid TuitionId { get; set; }

    // Hệ thống tự sinh tên buổi theo quy tắc "Buổi 01", "Buổi 02"...
    [Required]
    [StringLength(50)]
    public string SessionName { get; set; } = string.Empty;

    public Guid? LectureTypeId { get; set; } // Khóa ngoại đến LectureType

    // Tên bài học trong buổi học
    [StringLength(255)]
    public string? LessonName { get; set; }

    // Mục tiêu học tập sau buổi học
    [StringLength(255)]
    public string? Objective { get; set; }

    // Nội dung chi tiết giảng dạy
    [StringLength(255)]
    public string? Content { get; set; }

    // Bài tập về nhà cho học viên
    [StringLength(500)]
    public string? Homework { get; set; }

    // Tài liệu tham khảo (tên hoặc link)
    [StringLength(255)]
    public string? Reference { get; set; }

    // ===============================
    // Navigation properties
    // ===============================
    [ForeignKey("TuitionId")]
    //Course – CourseLesson: Một Course có nhiều CourseLesson, mỗi CourseLesson thuộc về một Course.
    public virtual Tuition? Tuition { get; set; }

    [ForeignKey("LectureTypeId")]
    //LectureType – CourseLesson: Một LectureType có thể gán cho nhiều CourseLesson, mỗi CourseLesson chỉ có một LectureType
    public virtual LectureType? LectureType { get; set; }

    [InverseProperty(nameof(Attachment.CourseLessonHomework))]
    public virtual ICollection<Attachment>? HomeworkAttachments { get; set; }

    [InverseProperty(nameof(Attachment.CourseLessonReference))]
    public virtual ICollection<Attachment>? ReferenceAttachments { get; set; }
}

