using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("LearningRoadMap")]
    public class LearningRoadMap : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string LearningRoadMapCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string LearningRoadMapName { get; set; } = string.Empty;
        public string? EnLearningRoadMapName { get; set; } = string.Empty;

        [MaxLength (1000)]
        public string? Description { get; set; }
        public string? EnDescription { get; set; }


        public Guid AgeGrId { get; set; }

        //sử dụng AgeGrId làm foreign key để liên kết với bảng Category
        [ForeignKey (nameof (AgeGrId))]
        public virtual Category? AgeGroup { get; set; } = null!;

        public virtual ICollection<Course>? Courses { get; set; }

        // Hải - Thêm trường Cam kết đầu ra kiểu bit
        public bool CommitmentOutput { get; set; } = false; // default false
        // Thứ tự chương trình kiểu int        
        public int Order { get; set; } = 0;
        public string? Icon { get; set; } // icon cho chương trình học
        public bool IsPublish { get; set; } = false; // Đăng trên website
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN

        public float VotingRate { get; set; } // Điểm đánh giá
        public int NumberOfStudents { get; set; } // Số lượng học viên đã đăng ký
        public int NumberOfSatisfiedStudents { get; set; } // Số lượng học viên hài lòng
        public virtual ICollection<Image> Images { get; set; } = new List<Image> ( );
    }
}
