
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class PromotionGroup : BaseEntity
{
    public string? Name { get; set; } //tên nhóm chương trình
    public string? Description { get; set; } //mô tả

}
