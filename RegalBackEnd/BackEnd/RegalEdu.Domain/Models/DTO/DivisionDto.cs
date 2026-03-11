namespace RegalEdu.Domain.Models.DTO
{
    public class DivisionDto
    {
        public Guid Id { get; set; }
        public required string DivisionCode { get; set; }
        public required string DivisionName { get; set; }
    }

}
