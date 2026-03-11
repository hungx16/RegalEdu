using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Company")]
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string CompanyCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string CompanyName { get; set; } = string.Empty;

        public string? EnCompanyName { get; set; }

        public string? Description { get; set; }

        public string? EnDescription { get; set; }

        [MaxLength (1000)]
        public string? CompanyAddress { get; set; }
        public string? EnCompanyAddress { get; set; }

        [MaxLength (20)]
        public string? CompanyPhone { get; set; }

        public DateTime? EstablishmentDate { get; set; }

        /// <summary>
        /// Mã tỉnh/thành theo ProvinceCode (không liên kết FK)
        /// </summary>
        [MaxLength (10)]
        public required string ProvinceCode { get; set; }


        /// <summary>
        /// FK đến người quản lý (employee)
        /// </summary>
        public Guid? ManagerId { get; set; }

        [ForeignKey ("ManagerId")]
        public Employee? Manager { get; set; } = null!;
        public bool IsPublish { get; set; } = true; // Có hiển thị trên website hay không
        public ICollection<LogRegionCom> LogRegionComs { get; set; } = new List<LogRegionCom> ( );
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee> ( );

        public double? Latitude { get; set; }   // -90..90
        public double? Longitude { get; set; }  // -180..180
        /// <summary>
        /// Danh sách các hình ảnh của công ty.
        /// </summary>
        public virtual ICollection<Image> CompanyImages { get; set; } = new List<Image> ( );
        public ICollection<LogEmployeePosition>? LogEmployeePositions { get; set; } = new List<LogEmployeePosition> ( );

        public virtual ICollection<Student> Students { get; set; } = new List<Student> ( );
        public virtual ICollection<RegisterStudy>? RegisterStudies { get; set; }
        public virtual ICollection<AdmissionsQuotaCompany>? AdmissionsQuotaCompanies { get; set; }
        public virtual ICollection<Promotion>? Promotions { get; set; }

        public virtual ICollection<CompanyLearningRoadMap>? CompanyLearningRoadMaps { get; set; }

        [MaxLength (10)]
        public string? WardCode { get; set; }

        public string? CompanyEmail { get; set; }

        public int NumberOfStudents { get; set; } // Số lượng
        public string? Convenience { get; set; }

        public string? EnConvenience { get; set; }

        [Column (TypeName = "decimal(18,1)")]
        public decimal VotingRate { get; set; } // Điểm đánh giá


        // Thiện - Thay vì quan hệ trực tiếp Teachers
        //public ICollection<CompanyTeacher>? CompanyTeachers { get; set; }

        public string? LeaningRoadMapTags { get; set; } // Tag đường dẫn học liệu
        public string? EnLeaningRoadMapTags { get; set; } // Tag đường dẫn học liệu
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN

        //public Guid? TeacherId { get; set; }
        public string? WorkingTime { get; set; }
        public string? EnWorkingTime { get; set; }
        public bool IsHeadQuarters { get; set; }
        public virtual ICollection<AllocationDetailEvent>? AllocationDetailEvents { get; set; } = new List<AllocationDetailEvent> ( );
    }

    //// 🔹 Quan hệ: 1 Company có nhiều AllocationDetailEvent




}
