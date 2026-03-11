using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("AllocationEventHistory")]
    public class AllocationEventHistory : BaseEntity
    {
        public Guid AllocationEventId { get; set; }
        public string? TargetName { get; set; }
        public string? ActionName { get; set; }
        public string? Description { get; set; }
    }

}
