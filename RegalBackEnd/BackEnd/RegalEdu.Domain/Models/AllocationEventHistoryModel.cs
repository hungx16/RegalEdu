namespace RegalEdu.Domain.Models
{
    public class AllocationEventHistoryModel: BaseEntityModel
    {
        public Guid Id { get; set; }
        public Guid AllocationEventId { get; set; }
        public string? TargetName { get; set; }
        public string? ActionName { get; set; }
        public string? Description { get; set; }
    }

}
