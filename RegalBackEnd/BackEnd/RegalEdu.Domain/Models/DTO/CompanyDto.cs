namespace RegalEdu.Domain.Models.DTO
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public string? ProvinceName { get; set; }
        public string? ManagerName { get; set; }
        public string? CompanyParentName { get; set; }
        public bool PublishOnTheWebsite { get; set; } = true; // Có hiển thị trên website hay không


        public string? CompanyEmail { get; set; }



    }
}
