using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class DiscountModel : BaseEntityModel
    {
        public int? Method { get; set; }
        public double? DiscountMax { get; set; }
        public Guid? PromotionId { get; set; }

        public virtual ICollection<DiscountDetailModel>? DiscountDetails { get; set; }
    }

}
