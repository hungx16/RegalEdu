using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Department.Commands
{
    public class RestoreListDepartmentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class RestoreListDepartmentCommandHandler : IRequestHandler<RestoreListDepartmentCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<RestoreListDepartmentCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public RestoreListDepartmentCommandHandler(IRegalEducationDbContext context, ILogger<RestoreListDepartmentCommandHandler> logger, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(RestoreListDepartmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, EntityName.Department));

            int successCount = 0, failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var department = await _context.Departments
                    .IgnoreQueryFilters ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);

                if (department == null)
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, EntityName.Department, id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                    continue;
                }
                if (!department.IsDeleted)
                {
                    failCount++;
                    var notDeletedMsg = _localizer.Format (LocalizationKey.EntityNotDeleted, EntityName.Department, department.Id);
                    failMessages.Add (notDeletedMsg);
                    continue;
                }

                department.IsDeleted = false;
                // department.DeletedAt = null;
                // department.DeletedBy = null;
                successCount++;
                _context.Departments.Update (department);
            }

            var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

            var mainMsg = _localizer.Format (
                LocalizationKey.MSG_RESTORE_RESULT,
                EntityName.Department, successCount, failCount);

            if (failMessages.Any ( ))
                mainMsg += " " + string.Join (" ", failMessages);

            if (dbResult && successCount > 0)
                return Result.Success (mainMsg);
            else
                return Result.Failure (mainMsg);
        }


    }
}
