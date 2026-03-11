
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class AllocationDetailEventModel : BaseEntityModel
    {
        // FK đến bảng AllocationEvent
        [ForeignKey ("AllocationEvent")]
        public Guid AllocationEventId { get; set; }
        public virtual AllocationEvent? AllocationEvent { get; set; }

        // FK đến bảng Event
        [ForeignKey ("Event")]
        public Guid EventId { get; set; }
        public virtual Event? Event { get; set; } // nếu có entity Event thì mở ra

        [Required]
        public int Quantity { get; set; } // Số lượng sự kiện

        [Column (TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; } // = AllocationEvent.EventBudget = Ngân sách cho 1 chi nhánh

        // FK đến bảng Company (chi nhánh)
        [ForeignKey ("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public NoAllocation NoAllocation { get; set; }

        // FK đến bảng Region
        [ForeignKey ("Region")]
        public Guid RegionId { get; set; }
        public virtual Region? Region { get; set; }

        public virtual List<CompanyEventModel>? CompanyEvents { get; set; }
    }
}
