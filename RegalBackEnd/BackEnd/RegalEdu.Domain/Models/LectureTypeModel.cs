using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models;

public class LectureTypeModel : BaseEntityModel
{
    [Required]
    [MaxLength (200)]
    public string LectureName { get; set; } = string.Empty; // Tên loại bài giảng

    [MaxLength (1000)]
    public string? Description { get; set; }         // Mô tả

    [MaxLength (255)]
    public string? FileUrl { get; set; }        // Đường dẫn file hướng dẫn (nếu có)

    //[MaxLength (255)]
    //public string? OriginalFileName { get; set; } // Sẽ lưu tên file gốc, ví dụ: "huong-dan.pdf"
    public AttachmentModel? Attachment { get; set; } // Navigation property đến Attachment
    public List<CourseLessonModel>? CourseLessons { get; set; } = new List<CourseLessonModel> ( );
}
