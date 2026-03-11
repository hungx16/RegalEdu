using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class RewardModel :BaseEntityModel
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Type { get; set; }

        [MaxLength(200)]
        public string? PromoCode { get; set; }

        /// <summary>
        /// 1 = Kích hoạt, 2 = Chưa kích hoạt
        /// </summary>
        public int Status { get; set; } = 1;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
