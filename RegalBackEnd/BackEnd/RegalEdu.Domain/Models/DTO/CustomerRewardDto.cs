using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Models.DTO
{
    public class CustomerRewardDto
    {
        public Guid Id { get; set; }
        public DateTime WonDate { get; set; }
        public string Prize { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid? RegionId { get; set; }
        public string? RegionName { get; set; }
        public Guid? LuckyDrawId { get; set; }
        public string? LuckyDrawName { get; set; }
        public Guid? RewardId { get; set; }
        public string? RewardName { get; set; }
        public Guid? ImageId { get; set; }
        public string? ImagePath { get; set; }
        public int ReceiveStatus { get; set; }
        public int AcceptanceStatus { get; set; }
        public string? Note { get; set; }
    }
}