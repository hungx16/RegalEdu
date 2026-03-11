using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.SupportingDocument.Commands
{
    public class RestoreListSupportingDocumentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
        public required List<WebsiteKey> WebsiteKeys { get; set; }

        public class RestoreListSupportingDocumentCommandHandler : IRequestHandler<RestoreListSupportingDocumentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<RestoreListSupportingDocumentCommandHandler> _logger;
            private readonly ILocalizationService _localizer;

            public RestoreListSupportingDocumentCommandHandler(
                IRegalEducationDbContext context,
                ILogger<RestoreListSupportingDocumentCommandHandler> logger,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _logger = logger ?? throw new ArgumentNullException (nameof (logger));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(RestoreListSupportingDocumentCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || !request.ListIds.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, "SupportingDocument"));

                int successCount = 0, failCount = 0;
                var fails = new List<string> ( );

                foreach (var id in request.ListIds)
                {
                    var entity = await _context.SupportingDocuments.IgnoreQueryFilters ( )
                        .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);

                    if (entity == null)
                    {
                        failCount++;
                        var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, "SupportingDocument", id);
                        fails.Add (msg); _logger.LogWarning (msg);
                        continue;
                    }
                    if (!entity.IsDeleted)
                    {
                        failCount++;
                        var msg = _localizer.Format (LocalizationKey.EntityNotDeleted, "SupportingDocument", entity.Id);
                        fails.Add (msg);
                        continue;
                    }

                    entity.IsDeleted = false;
                    //entity.DeletedAt = null;
                    //entity.DeletedBy = null;
                    _context.SupportingDocuments.Update (entity);
                    successCount++;
                }

                var db = await _context.SaveChangesAsync (cancellationToken) > 0;
                var summary = _localizer.Format (LocalizationKey.MSG_RESTORE_RESULT, "SupportingDocument", successCount, failCount);
                if (fails.Any ( )) summary += " " + string.Join (" ", fails);
                await WebsiteKeyHelper.SaveWebsiteKeysAsync (request.WebsiteKeys);
                return (db && successCount > 0) ? Result.Success (summary) : Result.Failure (summary);
            }
        }
    }
}
