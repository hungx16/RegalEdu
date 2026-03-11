namespace RegalEdu.Domain.Models.DTO
{
    public class PositionDto
    {
        public Guid Id { get; set; }
        public required string PositionCode { get; set; }
        public required string PositionName { get; set; }
        public bool? IsSale { get; set; } = false; // Có phải là nhân viên kinh doanh không

        public bool? IsSaleLead { get; set; } = false;
        public bool? IsSupport { get; set; } = false; // Có phải là quản lý không
    }
}
