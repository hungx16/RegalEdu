namespace RegalEdu.Domain.Models.DTO
{
    public class AdmissionsQuotaRegionDto
    {
        public Guid Id { get; set; }

        public Guid AdmissionsQuotaId { get; set; }
        public int CurrentExecutors { get; set; }          // tổng số lượng sale của vùng
        public int TotalSalesAllocated { get; set; }
        public Guid RegionId { get; set; }

        public decimal TotalQuota { get; set; }

        // --- Thông tin hiển thị (denormalized fields) ---
        public string? RegionName { get; set; }
        public int? AdmissionsQuotaYear { get; set; }
        public int? AdmissionsQuotaMonth { get; set; }
        public int OrderIndex { get; set; }   // NEW

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int CompanyCount { get; set; }
        public int CurrentSales { get; set; }
        public decimal RevenuePerSale { get; set; }
        public int NumberOfSalesAllocated { get; set; }
        public decimal TotalRevenue { get; set; }
        public RegionDto? Region { get; set; }
    }
}
