using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Employee.Queries
{
    public class IsMarketingEmployeeQuery : IRequest<Result<bool>>
    {
    }

    public class IsMarketingEmployeeQueryHandler : IRequestHandler<IsMarketingEmployeeQuery, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPermissionInfoService _permissionInfoService;

        public IsMarketingEmployeeQueryHandler(
            ICurrentUserService currentUserService,
            IUserPermissionInfoService permissionInfoService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _permissionInfoService = permissionInfoService ?? throw new ArgumentNullException(nameof(permissionInfoService));
        }

        public async Task<Result<bool>> Handle(IsMarketingEmployeeQuery request, CancellationToken cancellationToken)
        {
            var userCode = _currentUserService.UserCode;
            if (string.IsNullOrWhiteSpace(userCode))
            {
                return Result<bool>.Success(false);
            }

            var isMarketing = await _permissionInfoService.CheckUserIsMarketingAsync(userCode);
            return Result<bool>.Success(isMarketing);
        }
    }
}
