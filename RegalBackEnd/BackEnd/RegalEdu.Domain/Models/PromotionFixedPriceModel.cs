using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho Bảng chi tiết khuyến mại: áp dụng giá cố định
    public class PromotionFixedPriceModel : BaseEntityModel
    {
        public double? MinPrice { get; set; }
        public int? Limit { get; set; }
        public double? PriceSale { get; set; }
        public Guid? PromotionId { get; set; }
    }

}
