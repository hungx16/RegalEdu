using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    public class CustomerReward:BaseEntity
    {
        // Ngày trúng thưởng
        [Required]
        public DateTime WonDate { get; set; }

        // Tên giải thưởng học viên nhận
        [Required, MaxLength(200)]
        public string Prize { get; set; } = string.Empty;

        // Số điện thoại khách hàng
        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        // Họ và tên khách hàng
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        // Ngày sinh của khách hàng (không bắt buộc)
        public DateTime? BirthDate { get; set; }

        // Liên kết đến chi nhánh / company
        public Guid? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        // Liên kết đến vùng
        public Guid? RegionId { get; set; }
        [ForeignKey("RegionId")]
        public Region? Region { get; set; }

        // (Tùy chọn) liên kết tới chương trình quay số / đợt LuckyDraw
        public Guid? LuckyDrawId { get; set; }
        [ForeignKey("LuckyDrawId")]
        public LuckyDraw? LuckyDraw { get; set; }

        // (Tùy chọn) nếu giải thưởng là một entity riêng
        public Guid? RewardId { get; set; }
        [ForeignKey("RewardId")]
        public Reward? Reward { get; set; }

        // Hình ảnh báo cáo / bằng chứng (có thể lưu nhiều ảnh trong thực tế)
        public Guid? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public Image? Image { get; set; }

        // Trạng thái nhận quà: 1 = Chưa nhận, 2 = Đã xác nhận nhận quà (theo spec)
        public int ReceiveStatus { get; set; } = 1;

        // Trạng thái nghiệm thu: 1 = Chưa nghiệm thu, 2 = Đã nghiệm thu
        public int AcceptanceStatus { get; set; } = 1;

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
