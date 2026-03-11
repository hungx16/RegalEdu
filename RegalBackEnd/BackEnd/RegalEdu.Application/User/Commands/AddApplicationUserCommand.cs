using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.User.Commands
{
    public class AddApplicationUserCommand : IRequest<Result>
    {
        public required ApplicationUserModel ApplicationUserModel { get; set; }
    }

    public class AddApplicationUserCommandHandler : IRequestHandler<AddApplicationUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AddApplicationUserCommandHandler> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocalizationService _localizer;


        public AddApplicationUserCommandHandler(
            ILogger<AddApplicationUserCommandHandler> logger,
            IIdentityService identityService,
            AutoMapper.IMapper mapper,
            ICurrentUserService currentUserService,
            ILocalizationService localizer)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _localizer = localizer;
        }

        public async Task<Result> Handle(AddApplicationUserCommand request, CancellationToken cancellationToken)
        {
            const string model = "User";
            const string action = "Create";

            try
            {
                ApplicationUser user = _mapper.Map<ApplicationUser>(request.ApplicationUserModel);

                Result result = await _identityService.CreateUserAsync(user, request.ApplicationUserModel.Password);

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
