using RegalEdu.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class WorkBoardTeacherModel : BaseEntityModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        public TeacherModel? Teacher { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CheckinTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CheckoutTime { get; set; }

        [Required]
        [Range(1, 3)]
        public int Status { get; set; } = 1;

        public string? Location { get; set; }

        public string? Note { get; set; }

        public decimal WorkHours { get; set; }

        public bool IsConfirmed { get; set; } = false;

        public string? ConfirmedBy { get; set; }

        // Các thuộc tính tính toán
        public string StatusText => Status switch
        {
            1 => "Đúng giờ",
            2 => "Muộn",
            3 => "Vắng",
            _ => "Không xác định"
        };

        public TimeSpan? WorkingDuration => CheckinTime.HasValue && CheckoutTime.HasValue
            ? CheckoutTime.Value - CheckinTime.Value
            : null;
    }
}