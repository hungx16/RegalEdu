using RegalEdu.Domain.Models.DTO; 
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class ProfileModel : BaseEntityModel
    {
        // 10. Mã học viên
        [Required]
        [MaxLength(20)]
        public string StudentCode { get; set; } = string.Empty;

        // 11. Tên học viên
        [Required]
        [MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        // 12. Ngày hoàn thành thanh toán
        public DateTime? PaymentCompletedDate { get; set; }

        // 13. Hồ sơ học viên
        [MaxLength(50)]
        public string? ProfileType { get; set; }

        // 14. Chi nhánh (Company)
        [Required]
        public Guid CompanyId { get; set; }
        // Dựa theo EmployeeModel sử dụng CompanyDto
        public virtual CompanyDto? Company { get; set; }

        // 15. Vùng (Region)
        [Required]
        public Guid RegionId { get; set; }
        // Dựa theo EmployeeModel sử dụng RegionModel
        public virtual RegionModel? Region { get; set; }

        // 16. Nhân viên tư vấn (Employee)
        public Guid? ConsultantId { get; set; }
        // Sử dụng EmployeeModel cho thuộc tính điều hướng
        public virtual EmployeeModel? Consultant { get; set; }

        // 17. Trạng thái
        [MaxLength(30)]
        public string? ProfileStatus { get; set; }

        // Tham chiếu đến Học viên (Student)
        public Guid? StudentId { get; set; }
        public virtual StudentModel? Student { get; set; }
    }
}