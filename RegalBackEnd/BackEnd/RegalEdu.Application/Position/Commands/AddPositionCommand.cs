using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Commands
{
    public class AddPositionCommand : IRequest<Result>
    {
        public required PositionModel PositionModel { get; set; }
    }
    public class AddPositionCommandHandler : IRequestHandler<AddPositionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddPositionCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(AddPositionCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.Position> (request.PositionModel);
            await _context.Positions.AddAsync (entity, cancellationToken);

            var success = await _context.SaveChangesAsync (cancellationToken) > 0;
            if (success)
                return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Position));
            else
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Position));
        }
    }
}
