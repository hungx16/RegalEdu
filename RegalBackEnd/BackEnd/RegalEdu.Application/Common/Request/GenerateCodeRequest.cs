
namespace RegalEdu.Application.Common.Request
{
    public class GenerateCodeRequest
    {
        public string Prefix { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public int Length { get; set; } = 4;
        public string? Format { get; set; } = string.Empty;
        public int Year { get; set; } = 0;
        public int Month { get; set; } = 0;
    }

}
