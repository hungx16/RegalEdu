using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PromotionGroup.Commands
{
    public class DeleteListPromotionGroupCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListPromotionGroupCommandHandler : IRequestHandler<DeleteListPromotionGroupCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListPromotionGroupCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListPromotionGroupCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListPromotionGroupCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
            _softDeleteService = softDeleteService;
        }

        public async Task<Result> Handle(DeleteListPromotionGroupCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.PromotionGroup));

            int successCount = 0, failCount = 0;
            var failMessages = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _context.PromotionGroup.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.PromotionGroup));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format(
                            LocalizationKey.EntityDeleteFailed,
                            EntityName.PromotionGroup, id, result.Errors
                        );
                        failMessages.Add(deleteFailMsg);
                        _logger.LogWarning(deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format(
                        LocalizationKey.EntityWithIdNotFound,
                        EntityName.PromotionGroup, id
                    );
                    failMessages.Add(notFoundMsg);
                    _logger.LogWarning(notFoundMsg);
                }
            }

            var msg = _localizer.Format(
                LocalizationKey.MSG_DELETE_RESULT,
                EntityName.PromotionGroup, successCount, failCount
            );

            if (failMessages.Any())
                msg += " " + string.Join(" ", failMessages);

            return successCount > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
