using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Entities
{
    public class Reward:BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty; // Tên phần thưởng

        [MaxLength(100)]
        public string? Type { get; set; } // Loại phần thưởng (Học bổng/Tặng quà/...)

        // Mã chương trình khuyến mại hoặc mã coupon ứng với phần thưởng (nếu có)
        [MaxLength(200)]
        public string? PromoCode { get; set; }

        /// <summary>
        /// Trạng thái theo yêu cầu: 1 = Kích hoạt, 2 = Chưa kích hoạt
        /// (lưu dưới dạng int để phù hợp với UI combobox spec)
        /// </summary>
        public int Status { get; set; } = 1;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
