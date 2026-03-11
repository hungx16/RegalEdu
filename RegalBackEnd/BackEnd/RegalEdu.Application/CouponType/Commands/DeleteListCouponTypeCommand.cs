using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponType.Commands
{
    public class DeleteListCouponTypeCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListCouponTypeCommandHandler : IRequestHandler<DeleteListCouponTypeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListCouponTypeCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListCouponTypeCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListCouponTypeCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
            _softDeleteService = softDeleteService;
        }

        public async Task<Result> Handle(DeleteListCouponTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.CouponType));

            int successCount = 0, failCount = 0;
            var failMessages = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _context.CouponType.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.CouponType));
                    if (result.Succeeded) successCount++;
                    else
                    {
                        failCount++;
                        failMessages.Add(_localizer.Format(LocalizationKey.EntityDeleteFailed, EntityName.CouponType, id, result.Errors));
                    }
                }
                else
                {
                    failCount++;
                    failMessages.Add(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.CouponType, id));
                }
            }

            var msg = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, EntityName.CouponType, successCount, failCount);
            if (failMessages.Any()) msg += " " + string.Join(" ", failMessages);

            return successCount > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
