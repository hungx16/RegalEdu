using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Queries
{
    public class GetAllRegionsQuery : IRequest<Result<List<RegionModel>>>
    {
        public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, Result<List<RegionModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllRegionsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<RegionModel>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
            {
                var regions = await _context.Regions
                        .AsNoTracking ( ).AsSplitQuery ( )
                    .Include (r => r.Companies)
                    .Include (t => t.Manager).ThenInclude (t => t.ApplicationUser)

                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<RegionModel>> (regions);
                return Result<List<RegionModel>>.Success (result);
            }
        }
    }
}
