using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("WorkingTimeConfiguration")]
    public class WorkingTimeConfiguration : BaseEntity
    {
        [Required]
        [MaxLength (200)]
        public string NameConfiguration { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty; // Mô tả cấu hình ca làm việc

        public ICollection<WorkingTime> WorkingTimes { get; set; } = new List<WorkingTime> ( );
        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday> ( );

        public bool IsDefault { get; set; }
        public bool ApplyToSystem { get; set; } = true; // Có áp dụng cho toàn hệ thống hay không

        public ICollection<WorkingTimeConfigurationCompany>? WorkingTimeConfigurationCompanies { get; set; } = new List<WorkingTimeConfigurationCompany> ( );
    }
}
