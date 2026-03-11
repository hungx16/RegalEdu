using RegalEdu.Domain.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class PromotionGroupModel : BaseEntityModel
    {
        public string? Name { get; set; } //Tên nhóm chương trình
        public string? Description { get; set; } //Mô tả

    }
}
