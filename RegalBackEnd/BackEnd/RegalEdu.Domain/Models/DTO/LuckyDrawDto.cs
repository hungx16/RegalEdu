using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Models.DTO
{
   public class LuckyDrawDto:BaseEntityModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Branch { get; set; }
        public string? Region { get; set; }
        public DateTime? ReportDate { get; set; }
        public string? Reporter { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
