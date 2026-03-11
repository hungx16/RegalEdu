using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    public class AllocationEventModel : BaseEntityModel
    {
        [MaxLength (50)]
        public string AllocationCode { get; set; } = string.Empty; // Mã phân bổ

        [Range (1, 12)]
        public int AllocationMonth { get; set; } // Tháng phân bổ

        [Range (2000, 2100)]
        public int AllocationYear { get; set; } // Năm phân bổ 

        [Column (TypeName = "decimal(18,2)")]
        public decimal EventBudget { get; set; } // Ngân sách cho 1 chi nhánh

        public AllocationEventStatus AllocationEventStatus { get; set; } // Trạng thái (Nháp, Đã phân bổ, Hoàn thành, Huỷ)

        // 🔹 Quan hệ: Một AllocationEvent có nhiều AllocationDetailEvent
        public List<AllocationDetailEventModel> AllocationDetails { get; set; } = new List<AllocationDetailEventModel> ( );

        public List<AllocationEventHistoryModel> AllocationEventHistories { get; set; } = new List<AllocationEventHistoryModel> ( );

        public List<AllocationEventHistoryChange> HistoryChanges { get; set; } = new List<AllocationEventHistoryChange> ( );

    }

    public class AllocationEventHistoryChange
    {
        public required string ActionName { get; set; }
        public required string Description { get; set; }
        public required string TargetName { get; set; }

    }
}
