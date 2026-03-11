using MediatR;
using RegalEdu.Application.Common.Results;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PartnerType.Commands
{
    public class DeleteListPartnerTypeCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }

        public class Handler : IRequestHandler<DeleteListPartnerTypeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ILocalizationService _localizer;
            private readonly ISoftDeleteService _softDelete;

            public Handler(IRegalEducationDbContext context, ILogger<Handler> logger, ILocalizationService localizer, ISoftDeleteService softDelete)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
                _softDelete = softDelete ?? throw new ArgumentNullException(nameof(softDelete));
            }

            public async Task<Result> Handle(DeleteListPartnerTypeCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || !request.ListIds.Any())
                    return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, "PartnerType"));

                int success = 0, fail = 0;
                var fails = new List<string>();

                foreach (var id in request.ListIds)
                {
                    var entity = _context.PartnerTypes.FirstOrDefault(x => x.Id.ToString() == id);
                    if (entity == null)
                    {
                        fail++;
                        var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, "PartnerType", id);
                        fails.Add(msg); _logger.LogWarning(msg);
                        continue;
                    }

                    var res = await _softDelete.RecursiveSoftDelete(entity.Id, typeof(RegalEdu.Domain.Entities.PartnerType));
                    if (res.Succeeded) success++;
                    else
                    {
                        fail++;
                        var msg = _localizer.Format(LocalizationKey.EntityDeleteFailed, "PartnerType", id, res.Errors);
                        fails.Add(msg); _logger.LogWarning(msg);
                    }
                }

                var summary = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, "PartnerType", success, fail);
                if (fails.Any()) summary += " " + string.Join(" ", fails);
                return success > 0 ? Result.Success(summary) : Result.Failure(summary);
            }
        }
    }
}