using MediatR;
using RegalEdu.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.RegisterStudy.Commands
{
    public class RestoreListRegionCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }

        public class RestoreListRegionCommandHandler : IRequestHandler<RestoreListRegionCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<RestoreListRegionCommandHandler> _logger;
            private readonly ILocalizationService _localizer;

            public RestoreListRegionCommandHandler(
                IRegalEducationDbContext context,
                ILogger<RestoreListRegionCommandHandler> logger,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _logger = logger ?? throw new ArgumentNullException (nameof (logger));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(RestoreListRegionCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || !request.ListIds.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, "Region"));

                int successCount = 0;
                int failCount = 0;
                var failMessages = new List<string> ( );

                foreach (var id in request.ListIds)
                {
                    var entity = await _context.Regions.IgnoreQueryFilters ( ).FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);
                    if (entity == null)
                    {
                        failCount++;
                        var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, "Region", id);
                        failMessages.Add (notFoundMsg);
                        _logger.LogWarning (notFoundMsg);
                        continue;
                    }
                    if (!entity.IsDeleted)
                    {
                        failCount++;
                        var notDeletedMsg = _localizer.Format (LocalizationKey.EntityNotDeleted, "Region", entity.Id);
                        failMessages.Add (notDeletedMsg);
                        continue;
                    }
                    entity.IsDeleted = false;
                    //entity.DeletedAt = null;
                    //entity.DeletedBy = null;
                    successCount++;
                    _context.Regions.Update (entity);
                }

                var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

                string mainMsg = _localizer.Format (LocalizationKey.MSG_RESTORE_RESULT, "Region", successCount, failCount);
                if (failMessages.Any ( ))
                    mainMsg += " " + string.Join (" ", failMessages);

                if (dbResult && successCount > 0)
                    return Result.Success (mainMsg);
                else
                    return Result.Failure (mainMsg);
            }
        }
    }
}
