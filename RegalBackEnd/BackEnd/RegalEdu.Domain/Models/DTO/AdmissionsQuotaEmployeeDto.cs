using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Models.DTO
{
    public class AdmissionsQuotaEmployeeDto
    {
        public Guid Id { get; set; }

        public Guid AdmissionsQuotaId { get; set; }
        public string? AdmissionsQuotaName { get; set; }  // l?y t? navigation

        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        public Guid PositionId { get; set; }
        public string? PositionName { get; set; }

        public decimal? RevenueQuota { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? AllocationStartAt { get; set; }
        public DateTime? AllocationEndAt { get; set; }
        public int OrderIndex { get; set; }   // NEW

        public Guid? AdmissionsQuotaCompanyId { get; set; }
        public string? AdmissionsQuotaCompanyName { get; set; }

        public Guid? AdmissionsQuotaRegionId { get; set; }
        public string? AdmissionsQuotaRegionName { get; set; }

        public QuotaRole QuotaRole { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
