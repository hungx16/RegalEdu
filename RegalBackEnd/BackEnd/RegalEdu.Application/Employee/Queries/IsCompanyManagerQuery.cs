using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.Employee.Queries
{
    public class IsCompanyManagerQuery : IRequest<Result<bool>>
    {
    }

    public class IsCompanyManagerQueryHandler : IRequestHandler<IsCompanyManagerQuery, Result<bool>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public IsCompanyManagerQueryHandler(
            IRegalEducationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException (nameof (currentUserService));
        }

        public async Task<Result<bool>> Handle(IsCompanyManagerQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse (_currentUserService.EmployeeId, out var employeeId) || employeeId == Guid.Empty)
            {
                return Result<bool>.Success (false);
            }
            var companyManaged = await _context.Companies
                .AsNoTracking ( )
                .Where (r => r.ManagerId == employeeId && r.Status == StatusType.Active)
                .AnyAsync (cancellationToken);

            return Result<bool>.Success (companyManaged);
        }
    }
}
