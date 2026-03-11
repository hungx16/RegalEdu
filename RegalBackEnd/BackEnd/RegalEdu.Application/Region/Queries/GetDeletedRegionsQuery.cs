using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Queries
{
    public class GetDeletedRegionsQuery : IRequest<Result<List<RegionModel>>>
    {
        public class GetDeletedRegionsQueryHandler : IRequestHandler<GetDeletedRegionsQuery, Result<List<RegionModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedRegionsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<RegionModel>>> Handle(GetDeletedRegionsQuery request, CancellationToken cancellationToken)
            {
                var regions = await _context.Regions
                    .IgnoreQueryFilters ( )
                    .Include (r => r.Companies)
                    .Where (r => r.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<RegionModel>> (regions);
                return Result<List<RegionModel>>.Success (result);
            }
        }
    }
}
