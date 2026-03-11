using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class WorkingTimeConfigurationModel : BaseEntityModel
    {
        [Required]
        [MaxLength (200)]
        public string NameConfiguration { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty; // Mô tả cấu hình ca làm việc

        public ICollection<WorkingTimeDto> WorkingTimes { get; set; } = new List<WorkingTimeDto> ( );
        public ICollection<HolidayDto> Holidays { get; set; } = new List<HolidayDto> ( );

        public bool IsDefault { get; set; }

        public bool ApplyToSystem { get; set; } = true; // Có áp dụng cho toàn hệ thống hay không
        public ICollection<WorkingTimeConfigurationCompanyDto>? WorkingTimeConfigurationCompanies { get; set; } = new List<WorkingTimeConfigurationCompanyDto> ( );

    }
}
