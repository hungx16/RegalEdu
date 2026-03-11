using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RegalEdu.Domain.Entities
{
    [Table ("Teachers")]
    public class Teacher : BaseEntity
    {
        [MaxLength (100)]
        public string? TeacherNickname { get; set; } // Biệt danh
        [MaxLength (1000)]
        public string? TeacherQualifications { get; set; } // Trình độ chuyên môn
        [MaxLength (1000)]
        public string? TeacherSpecialization { get; set; } // Chuyên môn
        public WorkType WorkType { get; set; } = WorkType.FullTime; // Loại giáo viên {0 - full time 1- partime 2- hợp đồng}
        [MaxLength (2000)]
        public DateTime? JoinDate { get; set; } // Ngày bắt đầu công tác
        public string? PreferLevel { get; set; } = ""; // Mức độ ưu tiên (0-5)
        public bool TeachingOutside { get; set; } = false; // Có dạy ngoài không
        public bool TeacherAssistant { get; set; } = false; // Có là giáo viên trợ giảng không
        public bool IsOnline { get; set; } = false; // Có dạy online không
        //[Required]
        //[MaxLength (20)]
        //public required string TeacherCode { get; set; } // Mã giáo viên, định dạng GVxxxx    
        //[Required]
        //[MaxLength (50)]
        //public required string TeacherName { get; set; } // Tên giáo viên
        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; } = null!;

        public ICollection<Class>? Classes { get; set; }
        public virtual ICollection<RegisterStudy>? RegisterStudys { get; set; }

        // Thay vì quan hệ trực tiếp Companies
        // public ICollection<Company>? Companies { get; set; }

        //public virtual ICollection<CompanyTeacher>? CompanyTeachers { get; set; }
        public Guid CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        public virtual Company? Company { get; set; }
        public string? SubCompanyIds { get; set; } // VD: "1,2,3" Danh sách ID công ty con

        public virtual ICollection<EvaluateTeacher>? EvaluateTeachers { get; set; }
        public virtual ICollection<PayrollTeacher>? PayrollTeachers { get; set; }
        public virtual ICollection<WorkBoardTeacher>? WorkBoardTeachers { get; set; }
        public virtual ICollection<TeacherWorkLog>? TeacherWorkLogs { get; set; }
    }
}

