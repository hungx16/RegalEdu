using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("WorkingTime")]
    public class WorkingTime : BaseEntity
    {
        [Required]
        [MaxLength (200)]
        public string Name { get; set; } = string.Empty; // Tên ca làm việc

        [Required]
        public TimeSpan StartTime { get; set; }          // Giờ bắt đầu

        [Required]
        public TimeSpan EndTime { get; set; }            // Giờ kết thúc

        /// <summary>
        /// Thứ trong tuần (0 = Chủ nhật, 1 = Thứ 2, ..., 6 = Thứ 7)
        /// </summary>
        [Required]
        public byte DayOfWeek { get; set; }


        public bool IsWorkingDay { get; set; } = true; // Có phải là ngày làm việc hay không

        public Guid? WorkingTimeConfigurationId { get; set; }

        public WorkingTimeConfiguration? WorkingTimeConfiguration { get; set; }
    }
}
