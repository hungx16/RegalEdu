using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Request;
using Microsoft.AspNetCore.Identity;
using RegalEdu.Domain.Entities;
using Hangfire;

namespace RegalEdu.Application.Auth.Commands
{
    public class RequestPasswordResetCommand : IRequest<Result>
    {
        public required ForgotPasswordRequest ChangePasswordRequest { get; set; }
    }

    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RequestPasswordResetCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILocalizationService _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        // Update the constructor to fix the error
        public RequestPasswordResetCommandHandler(
           ILogger<RequestPasswordResetCommandHandler> logger,
           IIdentityService identityService, IConfiguration configuration, ILocalizationService localizer, IEmailTemplateService templateService, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager;
        }
        public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {

            var user = await _identityService.GetUserByIdentifierAsync(request.ChangePasswordRequest.UserName);
            if (user == null || !string.Equals(user.Email, request.ChangePasswordRequest.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(_localizer["InvalidUsernameOrEmail"]);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var clientBaseUrl = _configuration["ApiBaseUrl"];
            var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;


            var resetLink = $"{clientBaseUrl}/api/RegalEduManagement/Auth/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}&culture={Uri.EscapeDataString(culture)}";
           
            // Email model
            var model = new PasswordResetLinkEmailModel
            {
                UserName = user.UserName,
                ResetLink = resetLink
            };

            var body = await _templateService.RenderTemplateAsync("PasswordResetLink", model);

           //await _emailService.SendEmailAsync(user.Email, _localizer["PasswordResetLinkEmailSubject"], body);
            // Enqueue background job
           
            BackgroundJob.Enqueue<IEmailService>(emailService =>
               emailService.SendEmailAsync(user.Email, _localizer["PasswordResetLinkEmailSubject"], body, true));

            return Result.Success();
        }



    }
}