using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho Chi tiết quà tặng khuyến mại
    public class PromotionGiftDetailModel : BaseEntityModel
    {
        public int? GiftName { get; set; }
        public int? QuantityGift { get; set; }
        public Guid? PromotionGiftId { get; set; }
    }

}
