namespace RegalEdu.Domain.Models.DTO
{
    public class AdmissionsQuotaCompanyDto
    {
        public Guid Id { get; set; }

        // Core fields
        public int? CurrentExecutors { get; set; }
        public Guid AdmissionsQuotaId { get; set; }
        public Guid CompanyId { get; set; }
        public int NumberOfSalesAllocated { get; set; }
        public decimal RevenueQuotaPerEC { get; set; }
        public decimal TotalQuota { get; set; }
        public Guid AdmissionsQuotaRegionId { get; set; }

        // Denormalized display fields (từ navigation)
        public string? CompanyName { get; set; }
        public string? AdmissionsQuotaRegionName { get; set; }
        public int? AdmissionsQuotaYear { get; set; }
        public int? AdmissionsQuotaMonth { get; set; }
        public int OrderIndex { get; set; }   // NEW

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int? CurrentSales { get; set; }
        public decimal RevenuePerSale { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
