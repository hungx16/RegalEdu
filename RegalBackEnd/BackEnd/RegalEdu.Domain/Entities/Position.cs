using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Position")]
    public class Position : BaseEntity
    {
        [Required]
        [MaxLength (10)]
        public string PositionCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string PositionName { get; set; } = string.Empty;

        [MaxLength (1000)]
        public string? Description { get; set; }


        public bool? IsSale { get; set; } = false; // Có phải là nhân viên kinh doanh không

        public bool? IsSaleLead { get; set; } = false;
        public bool? IsSupport { get; set; } = false; // Có phải là quản lý không

        // Quan hệ với bảng liên kết phòng ban
        public ICollection<DepartmentPosition> DepartmentPositions { get; set; } = new List<DepartmentPosition> ( );
        public ICollection<LogEmployeePosition>? LogEmployeePositions { get; set; } = new List<LogEmployeePosition> ( );

    }
}
