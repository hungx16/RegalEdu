namespace RegalEdu.Domain.Models.DTO
{
    public class RegionDto
    {
        public Guid Id { get; set; }
        public required string RegionCode { get; set; }
        public required string RegionName { get; set; }
    }
}
