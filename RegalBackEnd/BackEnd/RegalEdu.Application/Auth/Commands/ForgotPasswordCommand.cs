using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
namespace RegalEdu.Application.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public required ForgotPasswordRequest ForgetPasswordRequest { get; set; }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILocalizationService _localizer;

        // Update the constructor to fix the error
        public ForgotPasswordCommandHandler(
           ILogger<ForgotPasswordCommandHandler> logger,
           IIdentityService identityService, IEmailTemplateService templateService, IEmailService emailService, ILocalizationService localizer)
        {
            _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _templateService = templateService;
            _emailService = emailService;
            _localizer = localizer;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Tìm người dùng theo username
                var user = await _identityService.GetUserByIdentifierAsync (request.ForgetPasswordRequest.Email);
                if (user == null)
                {
                    return _logger.LogAndFail (_localizer, "UserNotFound", request.ForgetPasswordRequest.UserName);
                }

                //  Tạo mật khẩu mới
                var (identityResult, newPassword) = await _identityService.GenerateNewPasswordAsync (user.Email);
                if (!identityResult.Succeeded)
                {
                    return _logger.LogAndFail (_localizer, "PasswordResetFailed");
                }

                //  Chuẩn bị model cho template
                var model = new ForgotPasswordRequest
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = newPassword
                };

                // Render template (tự lấy culture hiện tại)
                var body = await _templateService.RenderTemplateAsync ("ForgotPassword", model);

                // Gửi email
                await _emailService.SendEmailAsync (user.Email, _localizer["PasswordResetEmailSubject"], body);

                // Thành công
                return Result.Success ( );
            }
            catch (Exception ex)
            {
                return _logger.LogErrorAndFail (_localizer, ex, "UnexpectedError");
            }
            finally
            {
                _logger.LogInformation ("Đặt lại mật khẩu cho {UserName} bởi {Actor}",
                    request.ForgetPasswordRequest.UserName, request.ForgetPasswordRequest.UserName ?? "Unknown");
            }
        }
    }
}
