using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LearningRoadMap.Commands
{
    public class DeleteListLearningRoadMapsCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListLearningRoadMapsCommandHandler : IRequestHandler<DeleteListLearningRoadMapsCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListLearningRoadMapsCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListLearningRoadMapsCommandHandler(IRegalEducationDbContext context, ILogger<DeleteListLearningRoadMapsCommandHandler> logger, ILocalizationService localizer, ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListLearningRoadMapsCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer[EntityName.LearningRoadMap]));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var learningRoadMap = _context.LearningRoadMaps.FirstOrDefault (x => x.Id.ToString ( ) == id);
                if (learningRoadMap != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete (learningRoadMap.Id, typeof (Domain.Entities.LearningRoadMap));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format (
                            LocalizationKey.EntityDeleteFailed,
                            _localizer[EntityName.LearningRoadMap], learningRoadMap.LearningRoadMapName, result.Errors
                        );
                        failMessages.Add (deleteFailMsg);
                        _logger.LogWarning (deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (
                        LocalizationKey.EntityWithIdNotFound,
                        _localizer[EntityName.LearningRoadMap], id
                    );
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                }
            }

            // Thông điệp tổng hợp đã localize hoàn toàn
            var msg = _localizer.Format (
                LocalizationKey.MSG_DELETE_RESULT,
                _localizer[EntityName.LearningRoadMap], successCount, failCount
            ); // "Xóa LearningRoadMap: 3 thành công, 2 thất bại."
            if (failMessages.Any ( ))
                msg += "\n" + string.Join ("\n", failMessages);

            if (successCount > 0)
                return Result.Success (msg);
            else
                return Result.Failure (msg);
        }
    }
}