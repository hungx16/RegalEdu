using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.Auth.Commands
{
    public class AddUserCommand : IRequest<Result>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get;set; }     = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
             ILogger<AddUserCommandHandler> logger,
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            (Result result, Guid userId) = await _identityService.CreateUserAsync(request.UserName, request.Password, false, request.Email, request.FullName, request.DepartmentId);
            _logger.LogInformation("Create User done with userId {0}", userId);
            return result;
        }
    }
}
