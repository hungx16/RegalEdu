using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Queries
{
    public class GetAllCompanyRegionsQuery : IRequest<Result<List<LogRegionComModel>>>
    {
        public class GetAllCompanyRegionsQueryHandler : IRequestHandler<GetAllCompanyRegionsQuery, Result<List<LogRegionComModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllCompanyRegionsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<LogRegionComModel>>> Handle(GetAllCompanyRegionsQuery request, CancellationToken cancellationToken)
            {
                var regionCompanies = await _context.LogRegionComs
               .Include (c => c.Company).ThenInclude (c => c.Manager)
                    .Include (c => c.Region)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<LogRegionComModel>> (regionCompanies);
                return Result<List<LogRegionComModel>>.Success (result);
            }
        }
    }
}
