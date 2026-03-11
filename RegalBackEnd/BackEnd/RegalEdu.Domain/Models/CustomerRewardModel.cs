using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
  public class CustomerRewardModel: BaseEntityModel
    {
        [Required]
        public DateTime WonDate { get; set; }

        [Required, MaxLength(200)]
        public string Prize { get; set; } = string.Empty;

        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? RegionId { get; set; }

        public Guid? LuckyDrawId { get; set; }
        public Guid? RewardId { get; set; }

        public Guid? ImageId { get; set; }

        /// <summary>
        /// 1 = Chưa nhận, 2 = Đã xác nhận nhận quà
        /// </summary>
        public int ReceiveStatus { get; set; } = 1;

        /// <summary>
        /// 1 = Chưa nghiệm thu, 2 = Đã nghiệm thu
        /// </summary>
        public int AcceptanceStatus { get; set; } = 1;

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
