using MediatR;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CustomerReward.Commands
{
    public class ConfirmAcceptanceCustomerRewardCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required string ConfirmedBy { get; set; }
    }

    public class ConfirmAcceptanceCustomerRewardCommandHandler : IRequestHandler<ConfirmAcceptanceCustomerRewardCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public ConfirmAcceptanceCustomerRewardCommandHandler(IRegalEducationDbContext context, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(ConfirmAcceptanceCustomerRewardCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.CustomerReward>().FindAsync(new object[] { request.Id });
            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_NOTFOUND, _localizer["CustomerReward"]));

            if (entity.AcceptanceStatus == 2)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_ALREADY_ACCEPTED, _localizer["CustomerReward"]));

            entity.AcceptanceStatus = 2; // accepted
            entity.UpdatedBy = request.ConfirmedBy;
            _context.Update(entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["CustomerReward"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["CustomerReward"]));
        }
    }
}
