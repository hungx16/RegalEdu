using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("PartnerType")]
    [Index (nameof (PartnerTypeCode), IsUnique = true)] // Unique theo yêu cầu "Mã định danh duy nhất"

    public class PartnerTypeModel : BaseEntityModel
    {
        [Required]
        [MaxLength (50)]
        public string PartnerTypeCode { get; set; } = string.Empty;
        [Required]
        public string PartnerTypeName { get; set; } = string.Empty;

        [MaxLength (300)]
        public string? Description { get; set; }
        public string EnPartnerTypeName { get; set; } = string.Empty;
        public bool IsMultilingual { get; set; }

    }
}
