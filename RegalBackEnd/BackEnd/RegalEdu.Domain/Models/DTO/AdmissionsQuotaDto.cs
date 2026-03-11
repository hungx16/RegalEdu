using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Models.DTO
{
    public class AdmissionsQuotaDto
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public QuotaStatus QuotaStage { get; set; }

        public int? TotalSalesAllocated { get; set; }
        public int? CurrentExecutors { get; set; }
        public decimal TotalQuota { get; set; }
        public string? Note { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
