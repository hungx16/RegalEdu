using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Request;
using Microsoft.AspNetCore.Identity;
using RegalEdu.Domain.Entities;
using Hangfire;
using RegalEdu.Application.Common.Logging;
namespace RegalEdu.Application.Auth.Commands
{
    public class ConfirmPasswordResetCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Culture { get; set; } = string.Empty;
    }

    public class ConfirmPasswordResetCommandHandler : IRequestHandler<ConfirmPasswordResetCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<ConfirmPasswordResetCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILocalizationService _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        // Update the constructor to fix the error
        public ConfirmPasswordResetCommandHandler(
           ILogger<ConfirmPasswordResetCommandHandler> logger,
           IIdentityService identityService, IEmailTemplateService templateService, IEmailService emailService, ILocalizationService localizer, UserManager<ApplicationUser> userManager)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateService = templateService;
            _emailService = emailService;
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<Result> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cultureInfo = new System.Globalization.CultureInfo(request.Culture ?? "en-US");
                System.Globalization.CultureInfo.CurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.CurrentUICulture = cultureInfo;
                // Tìm người dùng theo username
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return _logger.LogAndFail(_localizer, "UserNotFound");
                }

                //  Tạo mật khẩu mới
                var newPassword = GenerateSecurePassword();
                Microsoft.AspNetCore.Identity.IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, request.Token, newPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                    return _logger.LogAndFail(_localizer, errors);
                }

                //  Chuẩn bị model cho template
                var model = new ForgotPasswordRequest
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = newPassword
                };

                // Render template (tự lấy culture hiện tại)
                var body = await _templateService.RenderTemplateAsync("ForgotPassword", model);

                // Gửi email
                //await _emailService.SendEmailAsync(user.Email, _localizer["PasswordResetEmailSubject"], body);
                BackgroundJob.Enqueue<IEmailService>(emailService =>
              emailService.SendEmailAsync(user.Email, _localizer["PasswordResetEmailSubject"], body, true));
                // Thành công
                // Build success HTML
                var html = BuildSuccessPage();

                return Result.Success(html);
            }
            catch (Exception ex)
            {
                _logger.LogErrorAndFail(_localizer, ex, "UnexpectedError");
                var html = BuildErrorPage();

                return Result.Failure(html);
            }
        }

        private string BuildErrorPage()
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>{_localizer["PasswordResetErrorTitle"]}</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
                        .error {{ color: red; font-size: 20px; }}
                    </style>
                </head>
                <body>
                    <h2 class='error'>{_localizer["PasswordResetError"]}</h2>
                    <p>{_localizer["PasswordResetError_TryAgain"]}</p>
                    <p>{_localizer["PasswordResetError_ContactSupport"]}</p>
                </body>
                </html>";
        }


        private string BuildSuccessPage()
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>{_localizer["PasswordResetSuccessTitle"]}</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
                        .success {{ color: green; font-size: 20px; }}
                    </style>
                </head>
                <body>
                    <h2 class='success'>{_localizer["PasswordResetSuccess"]}</h2>
                    <p>{_localizer["PasswordResetSuccess_EmailNotice"]}</p>
                    <p>{_localizer["PasswordResetSuccess_CloseWindow"]}</p>
                </body>
                </html>";
        }


        private string GenerateSecurePassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 12) + "!Aa1";
        }
    }
}
