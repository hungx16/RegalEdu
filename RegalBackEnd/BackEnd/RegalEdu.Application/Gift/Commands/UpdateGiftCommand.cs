using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Gift.Commands
{
    public class UpdateGiftCommand : IRequest<Result>
    {
        public required GiftModel GiftModel { get; set; }
    }

    public class UpdateGiftCommandHandler : IRequestHandler<UpdateGiftCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<UpdateGiftCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateGiftCommandHandler(
            IRegalEducationDbContext context,
            ILogger<UpdateGiftCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateGiftCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Gift
                .FirstOrDefaultAsync(x => x.Id == request.GiftModel.Id, cancellationToken);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Gift));

            _mapper.Map(request.GiftModel, entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Gift))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Gift));
        }
    }
}
