using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class ImageDto : BaseEntityModel
    {
        public new Guid Id { get; set; }
        /// <summary>
        /// FK: Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }

        [ForeignKey ("CompanyId")]
        public CompanyDto? Company { get; set; } = null!;

        /// <summary>
        /// Đường dẫn hoặc URL của file ảnh.
        /// </summary>
        [MaxLength (500)]
        public string? Path { get; set; } = string.Empty;

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
    }
}
