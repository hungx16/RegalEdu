using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Models
{
    public class LuckyDrawModel:BaseEntityModel
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Branch { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        public DateTime? ReportDate { get; set; }

        [MaxLength(150)]
        public string? Reporter { get; set; }

        public int Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
