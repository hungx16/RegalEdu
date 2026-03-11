using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Department")]
    public class Department : BaseEntity
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
        [ForeignKey ("DivisionId")]
        public virtual Division? Division { get; set; }



        [MaxLength (1000)]
        public string? Description { get; set; }

        public virtual ICollection<DepartmentPosition>? DepartmentPositions { get; set; } = null;

        public bool IsPublish { get; set; } = false; // Đăng trên website
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN


    }
}
