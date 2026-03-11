namespace RegalEdu.Domain.Entities
{
    public class DataLog
    {
        public Guid Id { get; set; }
        public required string MadeBy { get; set; }
        public DateTime ByWhen { get; set; }

        public required string TableName { get; set; }

        public required string Action { get; set; }

        public required string OriginalData { get; set; }
        public required string CurrentData { get; set; }
    }
}
