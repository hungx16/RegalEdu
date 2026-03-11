
using RegalEdu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Entities;

public class Gift : BaseEntity
{
    public string? Name { get; set; } //tên quà tặng
    public Double Prices { get; set; } //giá quà tặng
    public string? Description { get; set; } //mô tả

}
