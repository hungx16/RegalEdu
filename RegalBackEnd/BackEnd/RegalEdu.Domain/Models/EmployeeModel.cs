using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class EmployeeModel : BaseEntityModel
    {

        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUserModel? ApplicationUser { get; set; } = null!;

        [Required]
        public Guid CompanyId { get; set; }
        public virtual CompanyDto? Company { get; set; } = null!;

        [Required]
        public Guid PositionId { get; set; }
        public virtual PositionModel? Position { get; set; } = null!;
        [Required]

        public Guid DepartmentId { get; set; }
        public virtual DepartmentModel? Department { get; set; }

        [MaxLength (100)]
        public string? EmployeeTax { get; set; } // Mã số thuế

        public bool? IsSupport { get; set; } // Có phải support không
        public DateTime? EmployeeStartedDate { get; set; } // Ngày bắt đầu làm việc
        public DateTime? EmployeeEndDate { get; set; } // Ngày nghỉ việc
        public DateTime? EmployeeNewEndDate { get; set; } // Ngày kết thúc thử việc
        public bool OperationalSupportTeam { get; set; } = false; // Có phải nhân viên hỗ trợ vận hành không
        public string? PersonalEmail { get; set; }
        public ICollection<RegionModel>? Regions;
        public ICollection<CompanyDto>? Companies;
        public ICollection<LogEmployeePositionDto>? LogEmployeePositions { get; set; } = new List<LogEmployeePositionDto> ( );

        public virtual ICollection<RegisterStudyModel>? RegisterStudies { get; set; }
        public virtual ICollection<AdmissionsQuotaEmployeeModel>? AdmissionsQuotaEmployees { get; set; }
    }
}
