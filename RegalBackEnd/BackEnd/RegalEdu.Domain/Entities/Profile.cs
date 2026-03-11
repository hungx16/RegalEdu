using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table("Profiles")]
    public class Profile : BaseEntity
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

        // 14. Chi nhánh (Tham chiếu đến Company)
        // Cấu trúc này dựa trên Employee.CompanyId
        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        // 15. Vùng (Tham chiếu đến Region)
        // Cấu trúc này dựa trên các mối quan hệ trong Region.cs
        [Required]
        public Guid RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region? Region { get; set; }

        // 16. Nhân viên tư vấn (Tham chiếu đến Employee)
        // Cấu trúc này dựa trên ManagerId trong Company.cs và Region.cs
        public Guid? ConsultantId { get; set; }//
        [ForeignKey("ConsultantId")]
        public virtual Employee? Consultant { get; set; }

        // 17. Trạng thái
        [MaxLength(30)]
        public string? ProfileStatus { get; set; }

        // -----
        // Tham chiếu trực tiếp đến Student
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}