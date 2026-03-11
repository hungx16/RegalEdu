using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("Degree")]
    public class DegreeModel : BaseEntityModel // BaseEntity: Id, CreatedAt, CreatedBy,...
    {

        [Required]
        [MaxLength (200)]
        public string DegreeName { get; set; } = string.Empty; // Tên bằng cấp (ví dụ: Cử nhân, Tiến sĩ...)

        [MaxLength (1000)]
        public string? Description { get; set; }         // Mô tả (ví dụ: lĩnh vực, tiêu chuẩn...)

    }
}
