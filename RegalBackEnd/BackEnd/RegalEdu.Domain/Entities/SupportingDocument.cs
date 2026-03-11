using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("SupportingDocument")]
    public class SupportingDocument : BaseEntity
    {


        [Required]
        [MaxLength (500)]
        public string DocumentName { get; set; } = string.Empty; // Tên tài liệu
        public string EnDocumentName { get; set; } = string.Empty; // Tên tài liệu tiếng anh
        public string Description { get; set; } = string.Empty; // Mô tả tài liệu
        public string EnDescription { get; set; } = string.Empty; // Mô tả tài liệu tiếng anh
        [Required]
        public int DocumentTypeId { get; set; } // FK loại tài liệu

        [MaxLength (500)]
        public string? WebsiteKeys { get; set; } // Key trên website (cách nhau bởi dấu ;)
        public string? EnWebsiteKeys { get; set; } // Key trên website (cách nhau bởi dấu ;)
        public DateTime? StartDate { get; set; } // Ngày phát hành

        public DateTime? EndDate { get; set; } // Ngày kết thúc

        // THÊM THUỘC TÍNH NÀY
        [MaxLength (255)]
        public string? AuthorName { get; set; } // Tên tác giả của tài liệu
        public string? EnAuthorName { get; set; } // Tên tác giả của tài liệu
        public virtual Attachment? Attachment { get; set; } // Navigation property đến Attachment
        public virtual Image? Image { get; set; } = null!;

        public bool IsPublish { get; set; } = false;

        public FormatType Format { get; set; } = FormatType.Pdf;
        public TopicType Topic { get; set; } = TopicType.Grammar;
        public LevelType Level { get; set; } = LevelType.Beginner;

        public int YearRelease { get; set; }

        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN

        public string Link { get; set; } = "";
    }
}
