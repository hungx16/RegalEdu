using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.Teacher.Queries
{
    public class IsCurrentUserTeacherQuery : IRequest<Result<bool>>
    {
    }

    public class IsCurrentUserTeacherQueryHandler : IRequestHandler<IsCurrentUserTeacherQuery, Result<bool>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public IsCurrentUserTeacherQueryHandler(
            IRegalEducationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Result<bool>> Handle(IsCurrentUserTeacherQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.TeacherId) ||
                !Guid.TryParse(_currentUserService.TeacherId, out var teacherId))
            {
                return Result<bool>.Success(false);
            }

            var exists = await _context.Teachers
                .AsNoTracking()
                .Where(t => t.Id == teacherId && !t.IsDeleted && t.Status == StatusType.Active)
                .AnyAsync(cancellationToken);

            return Result<bool>.Success(exists);
        }
    }
}
