using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Employee.Queries
{
    public class IsAdmissionEmployeeQuery : IRequest<Result<bool>>
    {
    }

    public class IsAdmissionEmployeeQueryHandler : IRequestHandler<IsAdmissionEmployeeQuery, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPermissionInfoService _permissionInfoService;

        public IsAdmissionEmployeeQueryHandler(
            ICurrentUserService currentUserService,
            IUserPermissionInfoService permissionInfoService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _permissionInfoService = permissionInfoService ?? throw new ArgumentNullException(nameof(permissionInfoService));
        }

        public async Task<Result<bool>> Handle(IsAdmissionEmployeeQuery request, CancellationToken cancellationToken)
        {
            var userCode = _currentUserService.UserCode;
            if (string.IsNullOrWhiteSpace(userCode))
            {
                return Result<bool>.Success(false);
            }

            var isAdmission = await _permissionInfoService.CheckUserIsAdmissionAsync(userCode);
            return Result<bool>.Success(isAdmission);
        }
    }
}
