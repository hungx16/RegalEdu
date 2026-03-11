using RegalEdu.Domain.Enums;

namespace RegalEdu.Domain.Models.DTO
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string? EmployeeTax { get; set; }
        public string? FullName { get; set; } // Nếu có tên người dùng
        public string? CompanyName { get; set; } // Nếu cần
        public string? DepartmentName { get; set; } // Nếu cần
        public string? PositionName { get; set; } // Nếu cần

        public ApplicationUserModel? ApplicationUser { get; set; }
        public Guid CompanyId { get; set; }
        public StatusType Status { get; set; }
        public DateTime? EmployeeStartedDate { get; set; }
        public DateTime? EmployeeEndDate { get; set; }
        public PositionDto? Position { get; set; }
        public DateTime? EmployeeNewEndDate { get; set; }
    }
}
