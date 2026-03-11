
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Entities;

public class Publication : BaseEntity
{

    [Required]
    [MaxLength (50)]
    public string PublicationCode { get; set; } = string.Empty; // Mã ấn phẩm (Ví dụ: AP001)

    [Required]
    [MaxLength (200)]
    public string PublicationName { get; set; } = string.Empty; // Tên ấn phẩm

    [Required]
    public decimal UnitPrice { get; set; } // Đơn giá

    [Required]
    public int Quantity { get; set; } // Số lượng còn lại

}
