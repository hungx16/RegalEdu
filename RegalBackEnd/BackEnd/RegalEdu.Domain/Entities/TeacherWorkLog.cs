using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table("TeacherWorkLogs")]
    public class TeacherWorkLog : BaseEntity
    {
        public Guid TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher? Teacher { get; set; }

        public Guid? WorkingTimeId { get; set; }
        [ForeignKey(nameof(WorkingTimeId))]
        public virtual WorkingTime? WorkingTime { get; set; }

        public TeacherWorkType WorkType { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }

        public string? Note { get; set; }
    }
}
