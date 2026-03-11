namespace RegalEdu.Domain.Models.Request
{
    public class ReportRequest
    {
        public Enumerations.ReportCode ReportCode { get; set; }
        public List<ParamRequest>? ListParams { get; set; }
    }
    public class ParamRequest
    {
        public required string ParamKey { get; set; }
        public object? ParamValue { get; set; }
    }
}
