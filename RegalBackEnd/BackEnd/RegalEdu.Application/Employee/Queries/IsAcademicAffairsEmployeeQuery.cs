using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Employee.Queries
{
    public class IsAcademicAffairsEmployeeQuery : IRequest<Result<bool>>
    {
    }

    public class IsAcademicAffairsEmployeeQueryHandler : IRequestHandler<IsAcademicAffairsEmployeeQuery, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPermissionInfoService _permissionInfoService;

        public IsAcademicAffairsEmployeeQueryHandler(
            ICurrentUserService currentUserService,
            IUserPermissionInfoService permissionInfoService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _permissionInfoService = permissionInfoService ?? throw new ArgumentNullException(nameof(permissionInfoService));
        }

        public async Task<Result<bool>> Handle(IsAcademicAffairsEmployeeQuery request, CancellationToken cancellationToken)
        {
            var userCode = _currentUserService.UserCode;
            if (string.IsNullOrWhiteSpace(userCode))
            {
                return Result<bool>.Success(false);
            }

            var isAcademicAffairs = await _permissionInfoService.CheckUserIsAcademicAffairsAsync(userCode);
            return Result<bool>.Success(isAcademicAffairs);
        }
    }
}
