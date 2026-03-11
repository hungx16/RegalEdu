using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public required ChangePasswordRequest ChangePasswordRequest { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public ChangePasswordCommandHandler(
            ILogger<ChangePasswordCommandHandler> logger,
            IIdentityService identityService,
            ILocalizationService localizer)
        {
            _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace (request.ChangePasswordRequest?.UserName))
                {
                    return Result.Failure (_localizer["Auth.UserNameRequired"]);
                }

                if (string.IsNullOrWhiteSpace (request.ChangePasswordRequest?.OldPassword))
                {
                    return Result.Failure (_localizer["Auth.OldPasswordRequired"]);
                }

                if (string.IsNullOrWhiteSpace (request.ChangePasswordRequest?.NewPassword))
                {
                    return Result.Failure (_localizer["Auth.NewPasswordRequired"]);
                }

                var user = await _identityService.GetUserByIdentifierAsync (request.ChangePasswordRequest.UserName);
                if (user == null)
                {
                    return Result.Failure (_localizer.Format ("Auth.UserNotFound", request.ChangePasswordRequest.UserName));
                }

                var result = await _identityService.ChangePasswordAsync (
                    user.Id.ToString ( ),
                    request.ChangePasswordRequest.OldPassword,
                    request.ChangePasswordRequest.NewPassword
                );

                if (!result.Succeeded)
                {
                    return Result.Failure (_localizer["Auth.ChangePasswordFailed"]);
                }

                return Result.Success (_localizer["Auth.ChangePasswordSuccess"]);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizer.Format ("Auth.ChangePasswordException", ex.Message);
                if (ex.InnerException != null)
                {
                    errorMessage += $" | {_localizer.Format ("Common.InnerException", ex.InnerException.Message)}";
                }
                return Result.Failure (errorMessage);
            }
            finally
            {
                _logger.LogInformation (
                    _localizer.Format ("Auth.ChangePasswordLog",
                        request.ChangePasswordRequest?.UserName ?? "Unknown",
                        request.ChangePasswordRequest?.UserName ?? "Unknown"));
            }
        }
    }
}
