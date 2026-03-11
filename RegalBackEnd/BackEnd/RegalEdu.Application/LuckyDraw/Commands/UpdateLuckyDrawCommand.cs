using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LuckyDraw.Commands
{
    public class UpdateLuckyDrawCommand : IRequest<Result>
    {
        public required LuckyDrawModel LuckyDrawModel { get; set; }
    }

    public class UpdateLuckyDrawCommandHandler : IRequestHandler<UpdateLuckyDrawCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateLuckyDrawCommandHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateLuckyDrawCommand request, CancellationToken cancellationToken)
        {
            if (!request.LuckyDrawModel.Id.HasValue)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_INVALID_ID, _localizer["LuckyDraw"]));

            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.LuckyDraw>().FindAsync(new object[] { request.LuckyDrawModel.Id.Value });
            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_NOTFOUND, _localizer["LuckyDraw"]));

            _mapper.Map(request.LuckyDrawModel, entity);
            _context.Update(entity);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["LuckyDraw"]));
            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["LuckyDraw"]));
        }
    }
}
