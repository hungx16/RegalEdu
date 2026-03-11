using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

[Table("Course")]
public class Course : BaseEntity // N?u c¢ BaseEntity, s? c¢ Id, CreatedAt, CreatedBy...
{

    // Thu?c t¡nh
    [Required]
    [StringLength(50)]
    public string CourseCode { get; set; } = string.Empty;


    [Required]
    [StringLength(255)]
    public string CourseName { get; set; } = string.Empty;

    public float MinAvgScore { get; set; }// Di?m trung bnh t?i thi?u d? h?c viˆn du?c c“ng nh?n ho…n th…nh kh¢a h?c
    public int Sequence { get; set; }//Th? t? kh¢a h?c trong chuong trnh
    [MaxLength(1000)]
    public string? Description { get; set; }//M“ t? kh¢a h?c
    public CommitmentOutputType CommitmentOutputType { get; set; } = CommitmentOutputType.None;
    [MaxLength(100)]
    public string? CommitmentLevel { get; set; }// Trnh d? cam k?t    

    public Guid? LearningRoadMapId { get; set; }

    // d—ng 2 field string d? luu ID, c ch nhau b?i d?u ph?y
    [MaxLength(500)]
    public string? MidExamIds { get; set; } // VD: "1,2,3" Danh s ch n?i dung ki?m tra gi?a k

    [MaxLength(500)]
    public string? FinalExamIds { get; set; } // VD: "4,5,6" Danh s ch n?i dung ki?m tra cu?i k

    [MaxLength(255)]
    public string? Reference { get; set; } // T…i li?u tham kh?o
    public bool IsPublish { get; set; } = false; // Dang trˆn website
    public string? CourseContent { get; set; } // N?i dung kh¢a h?c
    public string? CourseKey { get; set; } // Di?m n?i b?t
    //[Required]
    //public CourseStatus CourseStatus { get; set; } = CourseStatus.Draft;
    // Navigation property cho kh¢a ngo?i
    // Kh¢a ngo?i
    [ForeignKey("LearningRoadMapId")]
    public virtual LearningRoadMap? LearningRoadMap { get; set; }
    public virtual ICollection<DetailRegisterStudy>? DetailRegisterStudies { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public float? OrdinalNumber { get; set; }
    //M?t Course s? c¢ nhi?u Tuition (1 kh¢a h?c v?i 1 lo?i chuong trnh c? th? s? c¢ 1 d•ng Tuition)
    //Course - CourseLesson: M?t Course c¢ nhi?u CourseLesson, m?i CourseLesson thu?c v? m?t Course.
    public virtual ICollection<Tuition>? Tuitions { get; set; }
    public bool IsMultilingual { get; set; }

    public string? EnCourseName { get; set; } = string.Empty;
    public string? EnDescription { get; set; } = string.Empty;
    public string? EnCourseContent { get; set; } = string.Empty;
    public string? EnCourseKey { get; set; } = string.Empty;

    public string? Duration { get; set; } // Th?i lu?ng kh¢a h?c
    public string? EnDuration { get; set; } = string.Empty;
    public int? NumberOfStudents { get; set; } // S? lu?ng h?c viˆn da dang ky

    [Column(TypeName = "decimal(18,1)")]

    public float? VotingRate { get; set; } // Di?m d nh gi 

}
