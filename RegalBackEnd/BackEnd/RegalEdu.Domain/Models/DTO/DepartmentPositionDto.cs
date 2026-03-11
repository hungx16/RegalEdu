

namespace RegalEdu.Domain.Models.DTO
{
    public class DepartmentPositionDto
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public PositionDto? Position { get; set; }
        public DepartmentDto? Department { get; set; }
    }
}
