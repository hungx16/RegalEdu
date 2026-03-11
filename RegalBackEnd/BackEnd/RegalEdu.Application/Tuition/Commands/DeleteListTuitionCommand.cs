using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Tuition.Commands
{
    public class DeleteListTuitionCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListTuitionCommandHandler : IRequestHandler<DeleteListTuitionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListTuitionCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListTuitionCommandHandler(IRegalEducationDbContext context, ILogger<DeleteListTuitionCommandHandler> logger, ILocalizationService localizer, ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListTuitionCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, EntityName.Tuition));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var tuition = _context.Tuition.FirstOrDefault (x => x.Id.ToString ( ) == id);
                if (tuition != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete (tuition.Id, typeof (Domain.Entities.Tuition));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format (LocalizationKey.EntityDeleteFailed, EntityName.Tuition, id, result.Errors);
                        failMessages.Add (deleteFailMsg);
                        _logger.LogWarning (deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, EntityName.Tuition, id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                }
            }

            var msg = _localizer.Format (LocalizationKey.MSG_DELETE_RESULT, EntityName.Tuition, successCount, failCount);
            if (failMessages.Any ( ))
                msg += " " + string.Join (" ", failMessages);

            if (successCount > 0)
                return Result.Success (msg);
            else
                return Result.Failure (msg);
        }
    }
}