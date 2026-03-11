using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace RegalEdu.Domain.Entities;

public class Contact : BaseEntity
{
    public string FullName { get; set; } = default!;     // Tên khách hàng
    public string? Phone { get; set; }// Số điện thoại
    public string? Email { get; set; }// Email
    public Gender Gender { get; set; } = Gender.Male; //giới tính
    public string? Address { get; set; } //địa chỉ
    public Relationship Relationship { get; set; } = Relationship.Father; //quan hệ với người liên hệ
    public string? Note { get; set; } //ghi chú

    public Guid? StudentId { get; set; }
    public string? Username { get; set; } //tên đăng nhập
    public virtual Student? Student { get; set; } = default!; // Nhân viên tư vấn phụ trách
    //liên kết tài khoản người dùng tạo liên hệ
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; } = null!;
}
