// Namespaces:
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Image")]
    public class Image : BaseEntity
    {
        /// <summary>
        /// Đường dẫn hoặc URL của file ảnh.
        /// </summary>
        [Required]
        [MaxLength (500)]
        public string Path { get; set; } = string.Empty;
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

        /// <summary>
        /// Navigation property để EF Core biết mối quan hệ.
        /// </summary>
        [ForeignKey ("CompanyId")]
        public Company? Company { get; set; } = null!;

        public Guid? AffiliatePartnerId { get; set; }

        public AffiliatePartner? AffiliatePartner { get; set; } = null!;


        // --- Foreign Key đến SupportingDocument ---
        public Guid? SupportingDocumentId { get; set; }
        [ForeignKey ("SupportingDocumentId")]
        public SupportingDocument? SupportingDocument { get; set; } = null!;
        public Guid? LearningRoadMapId { get; set; }
        [ForeignKey ("LearningRoadMapId")]
        public LearningRoadMap? LearningRoadMap { get; set; } = null!;


    }
}