using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponIssue.Commands
{
    public class DeleteListCouponCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListCouponCommandHandler : IRequestHandler<DeleteListCouponCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListCouponIssueCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListCouponCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListCouponIssueCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
            _softDeleteService = softDeleteService;
        }

        public async Task<Result> Handle(DeleteListCouponCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.CouponIssue));

            int successCount = 0, failCount = 0;
            var fails = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _context.Coupons.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.Coupon));
                    if (result.Succeeded) successCount++;
                    else { failCount++; fails.Add(_localizer.Format(LocalizationKey.EntityDeleteFailed, EntityName.CouponIssue, id, result.Errors)); }
                }
                else
                {
                    failCount++;
                    fails.Add(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.CouponIssue, id));
                }
            }

            var msg = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, EntityName.CouponIssue, successCount, failCount);
            if (fails.Any()) msg += " " + string.Join(" ", fails);

            return successCount > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
