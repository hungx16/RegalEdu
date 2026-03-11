using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LuckyDraw.Commands
{
    public class CreateLuckyDrawCommand : IRequest<Result>
    {
        public required LuckyDrawModel LuckyDrawModel { get; set; }
    }

    public class CreateLuckyDrawCommandHandler : IRequestHandler<CreateLuckyDrawCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public CreateLuckyDrawCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(CreateLuckyDrawCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<RegalEdu.Domain.Entities.LuckyDraw>(request.LuckyDrawModel);
            await _context.SetEntity<RegalEdu.Domain.Entities.LuckyDraw>().AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, _localizer["LuckyDraw"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["LuckyDraw"]));
        }
    }
}
