using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CustomerReward.Commands
{
    public class UpdateCustomerRewardCommand : IRequest<Result>
    {
        public required CustomerRewardModel CustomerRewardModel { get; set; }
    }

    public class UpdateCustomerRewardCommandHandler : IRequestHandler<UpdateCustomerRewardCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateCustomerRewardCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateCustomerRewardCommand request, CancellationToken cancellationToken)
        {
            if (!request.CustomerRewardModel.Id.HasValue)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_INVALID_ID, _localizer["CustomerReward"]));

            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.CustomerReward>().FindAsync(new object[] { request.CustomerRewardModel.Id.Value });
            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_NOTFOUND, _localizer["CustomerReward"]));

            _mapper.Map(request.CustomerRewardModel, entity);
            _context.Update(entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["CustomerReward"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["CustomerReward"]));
        }
    }
}
