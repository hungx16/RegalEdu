
namespace RegalEdu.Domain.Entities;

public class PlacementTest : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student? Student { get; set; }
    public int Listening { get; set; }
    public int Speaking { get; set; }
    public int Writing { get; set; }
    public int TotalScore { get; set; }
    public string? EvaluatorName { get; set; }
    public string? Comment { get; set; }
}
