using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class GiftModel : BaseEntityModel
    {
        public string? Name { get; set; } //tên quà tặng
        public Double Prices { get; set; } //giá quà tặng
        public string? Description { get; set; } //mô tả

    }
}
