using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Queries
{
    public class GetRegionByIdQuery : IRequest<Result<RegionModel>>
    {
        public required string Id { get; set; }

        public class GetRegionByIdQueryHandler : IRequestHandler<GetRegionByIdQuery, Result<RegionModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetRegionByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<RegionModel>> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
            {
                var region = await _context.Regions
                    .Include (x => x.Companies)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (region == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["Region"], request.Id);
                    return Result<RegionModel>.Failure (msg);
                }

                var result = _mapper.Map<RegionModel> (region);
                return Result<RegionModel>.Success (result);
            }
        }
    }
}
