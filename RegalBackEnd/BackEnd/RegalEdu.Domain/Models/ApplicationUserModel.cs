
using System.ComponentModel.DataAnnotations;

namespace RegalEdu.Domain.Models
{
    public class ApplicationUserModel
    {
        [Required, MaxLength (200)]
        public string FullName { get; set; } = string.Empty; // Họ tên

        public DateTime? DateOfBirth { get; set; } // Ngày sinh

        public bool Gender { get; set; } // true: Nam, false: Nữ, null: Khác

        [MaxLength (1000)]
        public string? Address { get; set; } // Địa chỉ cá nhân (thường trú)

        [MaxLength (200)]
        public string? ProvinceCode { get; set; } // Quê quán

        [MaxLength (12)]
        public string? IdentityNumber { get; set; } // CCCD/CMND

        [MaxLength (10)]
        public string? Nationality { get; set; } // Quốc tịch

        [MaxLength (2000)]
        public string? AvatarUrl { get; set; } // Ảnh đại diện

        public bool? IsDeleted { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo tài khoản
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public byte[]? Avatar { get; set; }
        public string UserCode { get; set; } = string.Empty; // Mã người dùng, có thể là mã định danh duy nhất
        public bool RequireChangePassword { get; set; }
        // public Student? Student { get; set; } // sau này mở rộng
        public string? PasswordConfirm { get; set; } = string.Empty;


        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? AvatarString { get; set; }
        public Guid? Id { get; set; }

        // Navigation properties (1-1 hoặc 1-n nếu cần)
        public EmployeeModel? Employee { get; set; }
        public TeacherModel? Teacher { get; set; }
        public ContactModel? Contact { get; set; }
        public string? GenderText { get; set; }
        public string? Note { get; set; } = string.Empty; // Ghi chú về người dùng, có thể là thông tin bổ sung hoặc lưu ý quan trọng
    }
}
