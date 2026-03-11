using MediatR;
using RegalEdu.Application.Common.Results;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Item.Commands
{
    public class DeleteListItemCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }

        public class Handler : IRequestHandler<DeleteListItemCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ILocalizationService _localizer;
            private readonly ISoftDeleteService _softDelete;

            public Handler(IRegalEducationDbContext context, ILogger<Handler> logger, ILocalizationService localizer, ISoftDeleteService softDelete)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _logger = logger ?? throw new ArgumentNullException (nameof (logger));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _softDelete = softDelete ?? throw new ArgumentNullException (nameof (softDelete));
            }

            public async Task<Result> Handle(DeleteListItemCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || !request.ListIds.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, "Item"));

                int success = 0, fail = 0; var fails = new List<string> ( );

                foreach (var id in request.ListIds)
                {
                    var entity = _context.Items.FirstOrDefault (x => x.Id.ToString ( ) == id);
                    if (entity == null)
                    {
                        fail++;
                        var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, "Item", id);
                        fails.Add (msg); _logger.LogWarning (msg);
                        continue;
                    }

                    var res = await _softDelete.RecursiveSoftDelete (entity.Id, typeof (RegalEdu.Domain.Entities.Item));
                    if (res.Succeeded) success++;
                    else
                    {
                        fail++;
                        var msg = _localizer.Format (LocalizationKey.EntityDeleteFailed, "Item", id, res.Errors);
                        fails.Add (msg); _logger.LogWarning (msg);
                    }
                }

                var summary = _localizer.Format (LocalizationKey.MSG_DELETE_RESULT, "Item", success, fail);
                if (fails.Any ( )) summary += " " + string.Join (" ", fails);
                return success > 0 ? Result.Success (summary) : Result.Failure (summary);
            }
        }
    }
}
