using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Category")]
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string CategoryName { get; set; } = string.Empty;
        public string EnCategoryName { get; set; } = string.Empty;
        /// <summary>
        /// 1 = Nhóm tuổi, 2 = Nguồn, 3 = Kỹ năng, 4 = Loại nghỉ lễ
        /// </summary>
        [Required]
        public byte CategoryType { get; set; }

        /// <summary>
        /// Dùng cho khoảng giá trị (ví dụ nhóm tuổi)
        /// </summary>
        public int? From { get; set; }

        public int? To { get; set; }

        [MaxLength (1000)]
        public string? Description { get; set; }

        // 1 AgeGroup có nhiều LearningRoadmap
        //[JsonIgnore] // 👈 tránh vòng lặp khi serialize
        public virtual ICollection<LearningRoadMap>? LearningRoadMaps { get; set; }

        // Quan hệ 1-nhiều: 1 nhóm tuổi có nhiều học sinh
        //[JsonIgnore]
        public virtual ICollection<Student>? Students { get; set; }

    }
}
