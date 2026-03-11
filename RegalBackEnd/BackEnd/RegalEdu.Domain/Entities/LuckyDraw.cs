using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Entities
{
   public class LuckyDraw:BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Branch { get; set; }        // hoặc BranchId nếu FK

        [MaxLength(100)]
        public string? Region { get; set; }        // hoặc RegionId nếu FK

        public DateTime? ReportDate { get; set; }

        [MaxLength(150)]
        public string? Reporter { get; set; }

        public int Status { get; set; }            // hoặc enum

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // thêm trường mô tả nếu cần
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
