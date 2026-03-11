using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities
{
    [Table ("Item")]
    public class Item : BaseEntity
    {
        public string ItemCode { get; set; } = string.Empty; // Mã ấn phẩm
        public string ItemName { get; set; } = string.Empty; // Tên ấn phẩm
        [Column (TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Đơn giá
        public int Quantity { get; set; } // Số lượng còn lại
    }
}
