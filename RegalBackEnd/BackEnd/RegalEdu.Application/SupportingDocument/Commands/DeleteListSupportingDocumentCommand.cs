using MediatR;
using RegalEdu.Application.Common.Results;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models.DTO;
using RegalEdu.Application.Common;

namespace RegalEdu.Application.SupportingDocument.Commands
{
    public class DeleteListSupportingDocumentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
        public required List<WebsiteKey> WebsiteKeys { get; set; }

        public class DeleteListSupportingDocumentCommandHandler : IRequestHandler<DeleteListSupportingDocumentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<DeleteListSupportingDocumentCommandHandler> _logger;
            private readonly ILocalizationService _localizer;
            private readonly ISoftDeleteService _softDeleteService;

            public DeleteListSupportingDocumentCommandHandler(
                IRegalEducationDbContext context,
                ILogger<DeleteListSupportingDocumentCommandHandler> logger,
                ILocalizationService localizer,
                ISoftDeleteService softDeleteService)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _logger = logger ?? throw new ArgumentNullException (nameof (logger));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
            }

            public async Task<Result> Handle(DeleteListSupportingDocumentCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || !request.ListIds.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer[EntityName.SupportingDocument]));

                int successCount = 0, failCount = 0;
                var fails = new List<string> ( );

                foreach (var id in request.ListIds)
                {
                    var entity = _context.SupportingDocuments.FirstOrDefault (x => x.Id.ToString ( ) == id);
                    if (entity == null)
                    {
                        failCount++;
                        var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.SupportingDocument], id);
                        fails.Add (msg); _logger.LogWarning (msg);
                        continue;
                    }

                    var result = await _softDeleteService.RecursiveSoftDelete (entity.Id, typeof (RegalEdu.Domain.Entities.SupportingDocument));
                    if (result.Succeeded) successCount++;
                    else
                    {
                        failCount++;
                        var msg = _localizer.Format (LocalizationKey.EntityDeleteFailed, _localizer[EntityName.SupportingDocument], entity.DocumentName, result.Errors);
                        fails.Add (msg); _logger.LogWarning (msg);
                    }
                }

                var summary = _localizer.Format (LocalizationKey.MSG_DELETE_RESULT, _localizer[EntityName.SupportingDocument], successCount, failCount);
                if (fails.Any ( )) summary += "\n" + string.Join ("\n", fails);
                await WebsiteKeyHelper.SaveWebsiteKeysAsync (request.WebsiteKeys);
                return successCount > 0 ? Result.Success (summary) : Result.Failure (summary);
            }
        }
    }
}
