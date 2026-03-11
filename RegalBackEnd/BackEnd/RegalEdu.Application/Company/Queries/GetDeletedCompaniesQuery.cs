using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Queries
{
    public class GetDeletedCompaniesQuery : IRequest<Result<List<CompanyModel>>>
    {
        public class GetDeletedCompaniesQueryHandler : IRequestHandler<GetDeletedCompaniesQuery, Result<List<CompanyModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedCompaniesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<CompanyModel>>> Handle(GetDeletedCompaniesQuery request, CancellationToken cancellationToken)
            {
                var companies = await _context.Companies
                    .IgnoreQueryFilters ( )
                    .Include (x => x.Manager).ThenInclude (x => x.ApplicationUser)
                    .Include (x => x.LogRegionComs).ThenInclude (x => x.Region)
                    .Where (x => x.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<CompanyModel>> (companies);
                return Result<List<CompanyModel>>.Success (result);
            }
        }
    }
}
