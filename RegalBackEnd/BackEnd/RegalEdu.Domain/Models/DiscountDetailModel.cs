using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{

    // Model cho Chi tiết chiết khấu
    public class DiscountDetailModel : BaseEntityModel
    {
        public double? MinAmount { get; set; }
        public int Limit { get; set; }
        public int DiscountType { get; set; }
        public double DiscountAmount { get; set; }
        public Guid? DiscountId { get; set; }
    }

}
