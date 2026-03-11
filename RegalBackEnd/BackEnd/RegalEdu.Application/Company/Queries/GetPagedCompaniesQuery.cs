using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Queries
{
    public class CompanyQuery
    {
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public string? ProvinceCode { get; set; }
        public Guid? ManagerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedCompaniesQuery : IRequest<Result<PagedResult<CompanyModel>>>
    {
        public CompanyQuery? CompanyQuery { get; set; }

        public class GetPagedCompaniesQueryHandler : IRequestHandler<GetPagedCompaniesQuery, Result<PagedResult<CompanyModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedCompaniesQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<CompanyModel>>> Handle(GetPagedCompaniesQuery request, CancellationToken cancellationToken)
            {
                if (request.CompanyQuery == null)
                    throw new ArgumentNullException (nameof (request.CompanyQuery));

                var query = _context.Companies
                    .Include (x => x.Manager).ThenInclude (x => x.ApplicationUser)
                    .Include (x => x.LogRegionComs).ThenInclude (x => x.Region)
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.CompanyQuery.CompanyCode))
                    query = query.Where (x => x.CompanyCode.Contains (request.CompanyQuery.CompanyCode));
                if (!string.IsNullOrWhiteSpace (request.CompanyQuery.CompanyName))
                    query = query.Where (x => x.CompanyName.Contains (request.CompanyQuery.CompanyName));
                if (!string.IsNullOrWhiteSpace (request.CompanyQuery.ProvinceCode))
                    query = query.Where (x => x.ProvinceCode.Contains (request.CompanyQuery.ProvinceCode));
                if (request.CompanyQuery.ManagerId.HasValue)
                    query = query.Where (x => x.ManagerId == request.CompanyQuery.ManagerId.Value);


                int totalRecords = await query.CountAsync (cancellationToken);
                request.CompanyQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.CompanyQuery.Page - 1) * request.CompanyQuery.PageSize)
                    .Take (request.CompanyQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<CompanyModel> (x)).ToList ( );

                var pagedResult = new PagedResult<CompanyModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<CompanyModel>>.Success (pagedResult);
            }
        }
    }
}
