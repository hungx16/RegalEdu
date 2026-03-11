using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Position.Commands
{
    public class DeleteListPositionCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }
    public class DeleteListPositionCommandHandler : IRequestHandler<DeleteListPositionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListPositionCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListPositionCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListPositionCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListPositionCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer[EntityName.Position]));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var entity = _context.Positions.FirstOrDefault (x => x.Id.ToString ( ) == id);
                if (entity != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete (entity.Id, typeof (RegalEdu.Domain.Entities.Position));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format (LocalizationKey.EntityDeleteFailed, _localizer[EntityName.Position], entity.PositionName, result.Errors);
                        failMessages.Add (deleteFailMsg);
                        _logger.LogWarning (deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.Position], id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                }
            }

            var msg = _localizer.Format (LocalizationKey.MSG_DELETE_RESULT, _localizer[EntityName.Position], successCount, failCount);
            if (failMessages.Any ( ))
                msg += "\n" + string.Join ("\n", failMessages);

            if (successCount > 0)
                return Result.Success (msg);
            else
                return Result.Failure (msg);
        }
    }
}
