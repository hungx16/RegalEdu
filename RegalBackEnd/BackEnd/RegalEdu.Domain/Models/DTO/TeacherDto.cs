namespace RegalEdu.Domain.Models.DTO
{
    public class TeacherDto
    {
        public Guid Id { get; set; }
        public required string TeacherCode { get; set; }
        public required string TeacherName { get; set; }
    }
}
