using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Employee.Commands
{
    public class RestoreListEmployeeCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }
    public class RestoreListEmployeeCommandHandler : IRequestHandler<RestoreListEmployeeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<RestoreListEmployeeCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public RestoreListEmployeeCommandHandler(
            IRegalEducationDbContext context,
            ILogger<RestoreListEmployeeCommandHandler> logger,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(RestoreListEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, "Employee"));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var entity = await _context.Employees.IgnoreQueryFilters ( ).FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);
                if (entity == null)
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, "Employee", id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                    continue;
                }
                if (!entity.IsDeleted)
                {
                    failCount++;
                    var notDeletedMsg = _localizer.Format (LocalizationKey.EntityNotDeleted, "Employee", entity.Id);
                    failMessages.Add (notDeletedMsg);
                    continue;
                }
                entity.IsDeleted = false;
                //  entity.DeletedAt = null;
                //  entity.DeletedBy = null;
                entity.ApplicationUser = await _context.ApplicationUsers.Where (t => t.Id == entity.ApplicationUserId).FirstOrDefaultAsync ( );
                // Khôi phục ApplicationUser nếu có liên kết
                if (entity.ApplicationUser != null)
                {
                    entity.ApplicationUser.IsDeleted = false;
                    entity.ApplicationUser.DeletedAt = null;
                    entity.ApplicationUser.DeletedBy = null;
                    _context.ApplicationUsers.Update (entity.ApplicationUser);  // Cập nhật ApplicationUser
                }
                successCount++;
                _context.Employees.Update (entity);
            }

            var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

            string mainMsg = _localizer.Format (LocalizationKey.MSG_RESTORE_RESULT, "Employee", successCount, failCount);
            if (failMessages.Any ( ))
                mainMsg += " " + string.Join (" ", failMessages);

            if (dbResult && successCount > 0)
                return Result.Success (mainMsg);
            else
                return Result.Failure (mainMsg);
        }
    }
}
