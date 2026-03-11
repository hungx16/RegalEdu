using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("RecruitmentInfo")]
    public class RecruitmentInfoModel : BaseEntityModel
    {
        [Required]
        [MaxLength (255)]
        public string RecruitmentInfoName { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength (100)]
        public string? Experience { get; set; }

        [Column (TypeName = "decimal(18,2)")]

        public decimal Salary { get; set; }


        public string Position { get; set; } = null!;

        [Column ("DepartmentId")]
        public Guid? DepartmentId { get; set; }

        [ForeignKey (nameof (DepartmentId))]
        public DepartmentModel? Department { get; set; }
        [MaxLength (200)]
        public string? ProvinceCode { get; set; } // Quê quán
        [MaxLength (255)]
        public string EnRecruitmentInfoName { get; set; } = null!;

        public string? EnDescription { get; set; }

        [MaxLength (100)]
        public string? EnExperience { get; set; }
        public string EnPosition { get; set; } = null!;

        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN
        public bool IsPublish { get; set; } = false;
        public WorkType WorkType { get; set; } = WorkType.FullTime;


    }
}
