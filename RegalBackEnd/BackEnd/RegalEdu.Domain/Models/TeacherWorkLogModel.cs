using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models;

public class TeacherWorkLogModel : BaseEntityModel
{
    public Guid TeacherId { get; set; }
    public Guid? WorkingTimeId { get; set; }
    public TeacherWorkType WorkType { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan StartTime { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan EndTime { get; set; }

    public string? Note { get; set; }
}
