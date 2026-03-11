using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class PositionModel : BaseEntityModel
    {
        [Required]
        [MaxLength (10)]
        public string PositionCode { get; set; } = string.Empty;

        [Required]
        [MaxLength (200)]
        public string PositionName { get; set; } = string.Empty;
        public bool? IsSale { get; set; } = false; // Có phải là nhân viên kinh doanh không

        public bool? IsSaleLead { get; set; } = false;
        public bool? IsSupport { get; set; } = false; // Có phải là quản lý không

        [MaxLength (1000)]
        public string? Description { get; set; }


        // Quan hệ với bảng liên kết phòng ban
        public ICollection<DepartmentPositionModel> DepartmentPositions { get; set; } = new List<DepartmentPositionModel> ( );

        public ICollection<LogEmployeePositionDto>? LogEmployeePositions { get; set; } = new List<LogEmployeePositionDto> ( );

    }
}
