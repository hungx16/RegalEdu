using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho bảng phiếu quà tặng
    public class PromotionCouponModel : BaseEntityModel
    {
        public double? MinQuantity { get; set; }
        public int? Limit { get; set; }
        public string? CouponCode { get; set; }
        public Guid? CouponTypeID { get; set; }
        public Guid? PromotionId { get; set; }
    }

}
