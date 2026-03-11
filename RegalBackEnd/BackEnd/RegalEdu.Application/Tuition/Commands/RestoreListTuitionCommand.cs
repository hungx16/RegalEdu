using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Tuition.Commands
{
    public class RestoreListTuitionCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class RestoreListTuitionCommandHandler : IRequestHandler<RestoreListTuitionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<RestoreListTuitionCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public RestoreListTuitionCommandHandler(IRegalEducationDbContext context, ILogger<RestoreListTuitionCommandHandler> logger, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(RestoreListTuitionCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, EntityName.Tuition));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var tuition = await _context.Tuition
                    .IgnoreQueryFilters ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);

                if (tuition == null)
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, EntityName.Tuition, id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                    continue;
                }

                if (!tuition.IsDeleted)
                {
                    failCount++;
                    var notDeletedMsg = _localizer.Format (LocalizationKey.EntityNotDeleted, EntityName.Tuition, tuition.Id);
                    failMessages.Add (notDeletedMsg);
                    continue;
                }

                tuition.IsDeleted = false;
                successCount++;
                _context.Tuition.Update (tuition);
            }

            var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

            string mainMsg = _localizer.Format (LocalizationKey.MSG_RESTORE_RESULT, EntityName.Tuition, successCount, failCount);
            if (failMessages.Any ( ))
                mainMsg += " " + string.Join (" ", failMessages);

            if (dbResult && successCount > 0)
            {
                return Result.Success (mainMsg);
            }
            else
            {
                return Result.Failure (mainMsg);
            }
        }
    }
}
