using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Holiday")]
    public class Holiday : BaseEntity
    {
        [Required]
        [MaxLength (200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// FK đến bảng Category – loại ngày nghỉ lễ (category_type = 4)
        /// </summary>
        public Guid? CategoryId { get; set; }

        [ForeignKey ("CategoryId")]
        public Category? Category { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }

        /// <summary>
        /// 0 = Lặp lại hàng năm, 1 = Không lặp lại
        /// </summary>
        public byte Frequency { get; set; } = 0;

        public Guid? WorkingTimeConfigurationId { get; set; }
        public WorkingTimeConfiguration? WorkingTimeConfiguration { get; set; }
    }
}
