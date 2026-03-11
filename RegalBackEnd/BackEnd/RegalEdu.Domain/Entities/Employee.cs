using System.ComponentModel.DataAnnotations;
namespace RegalEdu.Domain.Entities;

public class Employee : BaseEntity
{

    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; } = null!;

    [Required]
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; } = null!;

    [Required]
    public Guid PositionId { get; set; }
    public virtual Position? Position { get; set; } = null!;
    [Required]

    public Guid DepartmentId { get; set; }
    public virtual Department? Department { get; set; }

    [MaxLength (100)]
    public string? EmployeeTax { get; set; } // Mã số thuế

    public bool? IsSupport { get; set; } // Có phải support không
    public DateTime? EmployeeStartedDate { get; set; } // Ngày bắt đầu làm việc
    public DateTime? EmployeeEndDate { get; set; } // Ngày nghỉ việc
    public DateTime? EmployeeNewEndDate { get; set; } // Ngày kết thúc thử việc
    public string? PersonalEmail { get; set; }


    public ICollection<Region>? Regions;
    public ICollection<Company>? Companies;
    public ICollection<LogEmployeePosition>? LogEmployeePositions { get; set; } = new List<LogEmployeePosition> ( );
    public virtual ICollection<RegisterStudy>? RegisterStudies { get; set; }
    public virtual ICollection<AdmissionsQuotaEmployee>? AdmissionsQuotaEmployees { get; set; }
}
