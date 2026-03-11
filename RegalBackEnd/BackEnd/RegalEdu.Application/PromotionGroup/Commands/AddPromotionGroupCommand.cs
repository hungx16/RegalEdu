using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Commands
{
    public class AddPromotionGroupCommand : IRequest<Result>
    {
        public required PromotionGroupModel PromotionGroupModel { get; set; }
    }
    public class AddPromotionGroupCommandHandler : IRequestHandler<AddPromotionGroupCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<AddPromotionGroupCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddPromotionGroupCommandHandler(
            IRegalEducationDbContext context,
            ILogger<AddPromotionGroupCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(AddPromotionGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.PromotionGroup>(request.PromotionGroupModel);

            await _context.PromotionGroup.AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.PromotionGroup));

            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.PromotionGroup));
        }
    }
    
}
