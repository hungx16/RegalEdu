using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegalEdu.Domain.Models
{
   
    public class ContactModel : BaseEntityModel
    {
        public string FullName { get; set; } = default!;     // Tên khách hàng
        public string? Phone { get; set; }// Số điện thoại
        public string? Email { get; set; }// Email
        public Gender Gender { get; set; }  //giới tính
        public string? Address { get; set; } //địa chỉ
        public Relationship Relationship { get; set; } //quan hệ với người liên hệ
        public string? Note { get; set; } //ghi chú

        public Guid? StudentId { get; set; }
        public string? Username { get; set; } //tên đăng nhập
        public virtual StudentModel? Student { get; set; } = default!; // Nhân viên tư vấn phụ trách
                                                                  //liên kết tài khoản người dùng tạo liên hệ
        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUserModel? ApplicationUser { get; set; } = null!;
        public double? ExpectedBudget { get; set; }
    }
}
