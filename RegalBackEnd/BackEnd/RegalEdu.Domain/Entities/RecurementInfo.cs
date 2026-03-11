using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("RecruitmentInfo")]
    public class RecruitmentInfo : BaseEntity
    {
        [Required]
        [MaxLength (255)]
        public string RecruitmentInfoName { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength (100)]
        public string? Experience { get; set; }
        public string Position { get; set; } = null!;

        [Column (TypeName = "decimal(18,2)")]

        public decimal Salary { get; set; }

        [Column ("DepartmentId")]
        public Guid? DepartmentId { get; set; }

        [ForeignKey (nameof (DepartmentId))]
        public Department? Department { get; set; }


        public string ProvinceCode { get; set; } = null!;
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
