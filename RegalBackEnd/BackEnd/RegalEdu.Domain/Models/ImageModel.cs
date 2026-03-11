// Namespaces:
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("Image")]
    public class ImageModel : BaseEntityModel
    {
        /// <summary>
        /// Đường dẫn hoặc URL của file ảnh.
        /// </summary>
        [MaxLength (500)]
        public string? Path { get; set; } = string.Empty;
        [Required]
        [MaxLength (500)]
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// Chú thích cho ảnh (tùy chọn).
        /// </summary>
        [MaxLength (200)]
        public string? Caption { get; set; }

        /// <summary>
        /// Dùng để sắp xếp thứ tự hiển thị của các ảnh.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// Đánh dấu ảnh này có phải là ảnh bìa/ảnh đại diện hay không.
        /// </summary>
        public bool IsCover { get; set; } = false;

        // --- Foreign Key đến Company ---

        /// <summary>
        /// Khóa ngoại trỏ về Company.
        /// </summary>
        public Guid? CompanyId { get; set; }

        public Guid? AffiliatePartnerId { get; set; }
        // --- Foreign Key đến SupportingDocument ---
        public Guid? SupportingDocumentId { get; set; }

        public Guid? LearningRoadMapId { get; set; }

    }
}