
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;
[Table ("Event")]
public class Event : BaseEntity
{
    [Required]
    [MaxLength (10)]
    public string EventCode { get; set; } = string.Empty;//Mã sự kiện

    [Required]
    [MaxLength (200)]
    public string EventName { get; set; } = string.Empty;// Tên sự kiện   

    [MaxLength (1000)]
    public string? Description { get; set; } // Mô tả

    public EventCategory Category { get; set; }//Phân loại

    //Hải bổ sung 2609
    // 🔹 Quan hệ: 1 Event có thể xuất hiện trong nhiều AllocationDetailEvent
    public virtual ICollection<AllocationDetailEvent>? AllocationDetailEvents { get; set; } = new List<AllocationDetailEvent> ();
}
