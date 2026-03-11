using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    [Table ("Item")]
    public class ItemModel : BaseEntityModel
    {

        public string ItemCode { get; set; } = string.Empty; // Mã ấn phẩm
        public string ItemName { get; set; } = string.Empty; // Tên ấn phẩm
        public decimal Price { get; set; } // Đơn giá
        public int Quantity { get; set; } // Số lượng còn lại
    }
}
