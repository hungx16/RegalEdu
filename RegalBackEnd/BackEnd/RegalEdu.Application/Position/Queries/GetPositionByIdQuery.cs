using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Queries
{
    public class GetPositionByIdQuery : IRequest<Result<PositionModel>>
    {
        public required string Id { get; set; }
    }

    public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, Result<PositionModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetPositionByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<PositionModel>> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
        {
            var position = await _context.Positions
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (position == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Position"], request.Id);
                return Result<PositionModel>.Failure (msg);
            }

            var result = _mapper.Map<PositionModel> (position);
            return Result<PositionModel>.Success (result);
        }
    }
}
