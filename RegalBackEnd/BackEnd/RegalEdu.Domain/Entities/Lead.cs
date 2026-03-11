
namespace RegalEdu.Domain.Entities;

public class Lead : BaseEntity
{
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public required string Email { get; set; }
    public string? Source { get; set; }
    public string? AssignedTo { get; set; }
}
