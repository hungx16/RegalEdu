using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.User.Commands
{
    public class UpdateApplicationUserCommand : IRequest<Result>
    {
        public required ApplicationUserModel ApplicationUserModel { get; set; }
    }

    public class UpdateApplicationUserCommandHandler : IRequestHandler<UpdateApplicationUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<UpdateApplicationUserCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocalizationService _localizer;
        private readonly AutoMapper.IMapper _mapper;

        public UpdateApplicationUserCommandHandler(
            ILogger<UpdateApplicationUserCommandHandler> logger,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            ILocalizationService localizer,
            AutoMapper.IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _localizer = localizer;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateApplicationUserCommand request, CancellationToken cancellationToken)
        {
            const string model = "User";
            const string action = "Update";

            try
            {
                // 1️⃣ Load user từ DB
                var user = await _identityService.FindByIdAsync(request.ApplicationUserModel.Id ?? throw new ArgumentNullException(nameof(request.ApplicationUserModel.Id)));
                if (user == null)
                {
                    return Result.Failure(_localizer["UserNotFound"]);
                }

                // 2️⃣ Map từ model vào existing user
                _mapper.Map(request.ApplicationUserModel, user);

                // 3️⃣ Gọi UpdateUserAsync
                var (result, _) = await _identityService.UpdateUserAsync(user); // Deconstruct the tuple to extract the Result

                if (result.Succeeded)
                {
                    _logger.LogSuccess(action, model, _currentUserService.UserName);
                }
                else
                {
                    _logger.LogFail(action, model, _currentUserService.UserName, result.Errors);
                }

                return result;
            }
            catch (Exception ex)
            {
                return _logger.LogErrorAndFail(_localizer, ex, "UnexpectedError");
            }
        }
    }
}
