using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Commands
{
    public class UpdatePromotionGroupCommand : IRequest<Result>
    {
        public required PromotionGroupModel PromotionGroupModel { get; set; }
    }

    public class UpdatePromotionGroupCommandHandler : IRequestHandler<UpdatePromotionGroupCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<UpdatePromotionGroupCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdatePromotionGroupCommandHandler(
            IRegalEducationDbContext context,
            ILogger<UpdatePromotionGroupCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdatePromotionGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PromotionGroup
                .FirstOrDefaultAsync(x => x.Id == request.PromotionGroupModel.Id, cancellationToken);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.PromotionGroup));

            _mapper.Map(request.PromotionGroupModel, entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.PromotionGroup))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.PromotionGroup));
        }
    }
}
