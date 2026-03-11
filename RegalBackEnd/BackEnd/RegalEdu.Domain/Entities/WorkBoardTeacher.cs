using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    public class WorkBoardTeacher : BaseEntity
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } // Ngày làm việc

        [DataType(DataType.DateTime)]
        public DateTime? CheckinTime { get; set; } // Giờ checkin

        [DataType(DataType.DateTime)]
        public DateTime? CheckoutTime { get; set; } // Giờ checkout

        [Required]
        [Range(1, 3)]
        public int Status { get; set; } = 1; // 1: Đúng giờ, 2: Muộn, 3: Vắng

        [MaxLength(1000)]
        public string? Location { get; set; } // Địa điểm checkin (nếu có)

        // Các trường bổ sung có thể thêm
        [MaxLength(2000)]
        public string? Note { get; set; } // Ghi chú (lý do vắng, muộn, ...)

        [Column(TypeName = "decimal(4,2)")]
        public decimal WorkHours { get; set; } // Số giờ làm việc thực tế

        public bool IsConfirmed { get; set; } = false; // Đã xác nhận công chưa

        [MaxLength(50)]
        public string? ConfirmedBy { get; set; } // Người xác nhận
    }
}