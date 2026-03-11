using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    public class CompanyModel : BaseEntityModel
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
        public string CompanyPhone { get; set; } = string.Empty;

        public DateTime? EstablishmentDate { get; set; }

        /// <summary>
        /// FK đến tỉnh/thành
        /// </summary>
        public string? ProvinceCode { get; set; }



        /// <summary>
        /// FK đến người quản lý (employee)
        /// </summary>
        public Guid? ManagerId { get; set; }

        [ForeignKey ("ManagerId")]
        public EmployeeDto? Manager { get; set; }
        public bool IsPublish { get; set; } = true; // Có hiển thị trên website hay không
        public double? Latitude { get; set; }   // -90..90
        public double? Longitude { get; set; }  // -180..180
        public ICollection<LogRegionComDto>? LogRegionComs { get; set; } = new List<LogRegionComDto> ( );

        public virtual ICollection<EmployeeDto> Employees { get; set; } = new List<EmployeeDto> ( );
        public virtual ICollection<ImageDto> CompanyImages { get; set; } = new List<ImageDto> ( );
        public List<string>? DeletedImageIds { get; set; }
        public ICollection<LogEmployeePositionDto>? LogEmployeePositions { get; set; } = new List<LogEmployeePositionDto> ( );
        public virtual ICollection<RegisterStudyModel>? RegisterStudies { get; set; }

        public virtual ICollection<CompanyLearningRoadMapModel>? CompanyLearningRoadMaps { get; set; }


        [MaxLength (10)]
        public string? WardCode { get; set; }

        public string? CompanyEmail { get; set; }

        public int NumberOfStudents { get; set; } // Số lượng
        public string? Convenience { get; set; }
        public string? EnConvenience { get; set; }
        [Column (TypeName = "decimal(18,1)")]
        public decimal VotingRate { get; set; } // Điểm đánh giá


        // Thiện
        //public virtual ICollection<CompanyTeacherModel>? CompanyTeachers { get; set; }
        public string? LeaningRoadMapTags { get; set; } // Tag đường dẫn học liệu
        public string? EnLeaningRoadMapTags { get; set; } // Tag đường dẫn học liệu
        public bool IsMultilingual { get; set; } = false;  // true = có bản dịch EN
        public string? WorkingTime { get; set; }
        public string? EnWorkingTime { get; set; }
        public bool IsHeadQuarters { get; set; }
        public virtual ICollection<RegisterStudyModel>? RegisterStudys { get; set; }
        //Hải bổ sung 2609
        //// 🔹 Quan hệ: 1 Company có nhiều AllocationDetailEvent
        public List<AllocationDetailEventModel>? AllocationDetailEvents { get; set; } = new List<AllocationDetailEventModel> ( );

    }






}
