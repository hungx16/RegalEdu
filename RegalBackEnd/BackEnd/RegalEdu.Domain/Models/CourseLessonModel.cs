using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models;

public class CourseLessonModel : BaseEntityModel
{

    // Khóa ngoại đến bảng Tuition
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

    public List<AttachmentModel>? HomeworkAttachments { get; set; }

    public List<AttachmentModel>? ReferenceAttachments { get; set; }


    //Course – CourseLesson: Một Course có nhiều CourseLesson, mỗi CourseLesson thuộc về một Course.
    public virtual TuitionModel? Tuition { get; set; }

    //LectureType – CourseLesson: Một LectureType có thể gán cho nhiều CourseLesson, mỗi CourseLesson chỉ có một LectureType
    public virtual LectureTypeModel? LectureType { get; set; }
}




