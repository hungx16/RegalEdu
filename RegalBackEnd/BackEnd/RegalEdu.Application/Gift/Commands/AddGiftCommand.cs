using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Gift.Commands
{
    public class AddGiftCommand : IRequest<Result>
    {
        public required GiftModel GiftModel { get; set; }
    }

    public class AddGiftCommandHandler : IRequestHandler<AddGiftCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<AddGiftCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddGiftCommandHandler(
            IRegalEducationDbContext context,
            ILogger<AddGiftCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(AddGiftCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.Gift>(request.GiftModel);

            await _context.Gift.AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Gift))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Gift));
        }
    }
}
