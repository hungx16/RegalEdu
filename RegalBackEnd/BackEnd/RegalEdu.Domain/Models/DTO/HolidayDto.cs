using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models.DTO
{

    public class HolidayDto : BaseEntityModel
    {
        [Required]
        [MaxLength (200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// FK đến bảng Category – loại ngày nghỉ lễ (category_type = 4)
        /// </summary>
        public Guid? Type { get; set; }


        [MaxLength (1000)]
        public string? Description { get; set; }

        /// <summary>
        /// 0 = Lặp lại hàng năm, 1 = Không lặp lại
        /// </summary>
        public byte Frequency { get; set; } = 0;

        public Guid? WorkingTimeConfigurationId { get; set; }

    }
}
