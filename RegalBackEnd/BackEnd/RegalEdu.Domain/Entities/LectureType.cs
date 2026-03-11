using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("LectureType")]
    public class LectureType : BaseEntity
    {

        [Required]
        [MaxLength (200)]
        public string LectureName { get; set; } = string.Empty; // Tên loại bài giảng

        [MaxLength (1000)]
        public string? Description { get; set; }         // Mô tả

        [MaxLength (255)]
        public string? FileUrl { get; set; }        // Đường dẫn file hướng dẫn (nếu có)

        // THÊM THUỘC TÍNH NÀY
        //[MaxLength (255)]
        //public string? OriginalFileName { get; set; } // Sẽ lưu tên file gốc, ví dụ: "huong-dan.pdf"

        public virtual Attachment? Attachment { get; set; } // Navigation property đến Attachment
        //LectureType – CourseLesson: Một LectureType có thể gán cho nhiều CourseLesson, mỗi CourseLesson chỉ có một LectureType
        public virtual ICollection<CourseLesson>? CourseLessons { get; set; }
    }
}
