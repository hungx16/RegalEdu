using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RegalEdu.Application.Common.Request
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType (DataType.Password)]
        [StringLength (100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string? UserName { get; set; }
        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }
    }
    public class RegisterRequest
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength (100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType (DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType (DataType.Password)]
        [Compare ("Password", ErrorMessage = "The password and confirmation password do not match.")]

        [Required]

        public string ConfirmPassword { get; set; } = string.Empty;

    }

    public class RefreshTokenRequest
    {
        [JsonPropertyName ("refreshToken")]
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;
    }

    //public class VerifyTokenRequest
    //{

    //    public required string AccessToken { get; set; }

    //    public required string RefreshToken { get; set; }
    //}

    public class RequestData
    {
        public string? AccessToken { get; set; }
    }

    public class AuthResetModel
    {
        public string? UserName { get; set; }
        public string? Code { get; set; }

        public string? NewPassword { get; set; }

    }
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
    public class PasswordResetLinkEmailModel
    {
        public required string UserName { get; set; }
        public required string ResetLink { get; set; }
    }

    public class UserModelRequest
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
