namespace RegalEdu.Domain.Models.DTO
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public required string DepartmentCode { get; set; }
        public required string DepartmentName { get; set; }

        public string EnDepartmentName { get; set; } = string.Empty;
        public DivisionDto? Division { get; set; } // Phải đúng kiểu

        public bool IsPublish { get; set; } = false; // Đăng trên website
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN
    }
}
