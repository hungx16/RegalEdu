using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class TeacherModel : BaseEntityModel
{
    [MaxLength (100)]
    public string? TeacherNickname { get; set; }

    [MaxLength (1000)]
    public string? TeacherQualifications { get; set; }

    [MaxLength (1000)]
    public string? TeacherSpecialization { get; set; }

    public WorkType WorkType { get; set; } = (WorkType)1;
    public DateTime? JoinDate { get; set; }
    public string? PreferLevel { get; set; } = "";
    public bool TeachingOutside { get; set; } = false;
    public bool TeacherAssistant { get; set; } = false;
    public bool IsOnline { get; set; } = false;
    //public Guid ApplicationUserId { get; set; }
    public Guid? ApplicationUserId { get; set; }
    public Guid CompanyId { get; set; }
    [ForeignKey ("CompanyId")]
    public virtual CompanyModel? Company { get; set; }

    public string? SubCompanyIds { get; set; } // VD: "1,2,3" Danh sách ID công ty con
    public ApplicationUserModel? ApplicationUser { get; set; } = null!;

    // Thông tin nghiệp vụ (ví dụ như dạy những lớp nào)
    public ICollection<ClassModel>? Classes { get; set; }
    public virtual ICollection<RegisterStudyModel>? RegisterStudys { get; set; }
    public ICollection<CompanyModel>? Companies { get; set; }
}
