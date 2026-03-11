using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
    // Model cho bảng chiết khấu
    public class CouponTypeCouponModel : BaseEntityModel
    {
        public double? MinQuantity { get; set; } // phương thức quà tặng
        public int? Limit { get; set; }  //Số lượng quà tặng
        public string? CouponCode { get; set; } //khóa ngoại bảng Promotion
        public Guid? CouponTypeID { get; set; } //khóa ngoại bảng Promotion
        [ForeignKey("CouponTypeID")]
        public virtual CouponTypeModel? CouponType { get; set; } //chương trình khuyến mại

    }

}
