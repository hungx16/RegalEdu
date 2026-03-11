using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{

    public class CategoryModel : BaseEntityModel
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
        public List<LearningRoadMapModel>? LearningRoadMaps { get; set; } = new List<LearningRoadMapModel> ( );
        public List<StudentModel>? Students { get; set; } = new List<StudentModel> ( );
    }
}
