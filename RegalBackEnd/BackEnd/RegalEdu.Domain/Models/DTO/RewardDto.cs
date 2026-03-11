using System;

namespace RegalEdu.Domain.Models.DTO
{
    public class RewardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? PromoCode { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
    }
}