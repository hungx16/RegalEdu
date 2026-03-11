using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class DepartmentModel : BaseEntityModel
    {
        [Required]
        [MaxLength (10)]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string EnDepartmentName { get; set; } = string.Empty;
        /// <summary>
        /// FK: Khối tổ chức (Division)
        /// </summary>
        [Required]
        public Guid DivisionId { get; set; }
        public DivisionDto? Division { get; set; } // Phải đúng kiểu


        [MaxLength (1000)]
        public string? Description { get; set; }

        public virtual ICollection<DepartmentPositionModel>? DepartmentPositions { get; set; } = null;

        public bool IsPublish { get; set; } = false; // Đăng trên website
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN


    }
}
