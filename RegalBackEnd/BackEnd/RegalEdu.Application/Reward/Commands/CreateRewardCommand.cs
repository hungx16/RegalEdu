using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Reward.Commands
{
    public class CreateRewardCommand : IRequest<Result>
    {
        public required RewardModel RewardModel { get; set; }
    }

    public class CreateRewardCommandHandler : IRequestHandler<CreateRewardCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public CreateRewardCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(CreateRewardCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<RegalEdu.Domain.Entities.Reward>(request.RewardModel);
            await _context.SetEntity<RegalEdu.Domain.Entities.Reward>().AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Reward"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Reward"]));
        }
    }
}
