using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Reward.Commands
{
    public class UpdateRewardCommand : IRequest<Result>
    {
        public required RewardModel RewardModel { get; set; }
    }

    public class UpdateRewardCommandHandler : IRequestHandler<UpdateRewardCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateRewardCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateRewardCommand request, CancellationToken cancellationToken)
        {
            if (!request.RewardModel.Id.HasValue)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_INVALID_ID, _localizer["Reward"]));

            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.Reward>().FindAsync(new object[] { request.RewardModel.Id.Value });
            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_NOTFOUND, _localizer["Reward"]));

            _mapper.Map(request.RewardModel, entity);
            _context.Update(entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Reward"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Reward"]));
        }
    }
}
