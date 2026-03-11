using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Queries
{
    public class RegionQuery
    {
        public string? RegionCode { get; set; }
        public string? RegionName { get; set; }
        public Guid? ManagerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedRegionsQuery : IRequest<Result<PagedResult<RegionModel>>>
    {
        public RegionQuery? RegionQuery { get; set; }

        public class GetPagedRegionsQueryHandler : IRequestHandler<GetPagedRegionsQuery, Result<PagedResult<RegionModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedRegionsQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<RegionModel>>> Handle(GetPagedRegionsQuery request, CancellationToken cancellationToken)
            {
                if (request.RegionQuery == null)
                    throw new ArgumentNullException (nameof (request.RegionQuery));

                var query = _context.Regions
                    .Include (r => r.Companies)
                    .Include (t => t.Manager).ThenInclude (t => t.ApplicationUser)
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.RegionQuery.RegionCode))
                    query = query.Where (r => r.RegionCode.Contains (request.RegionQuery.RegionCode));
                if (!string.IsNullOrWhiteSpace (request.RegionQuery.RegionName))
                    query = query.Where (r => r.RegionName.Contains (request.RegionQuery.RegionName));
                if (request.RegionQuery.ManagerId.HasValue)
                    query = query.Where (r => r.ManagerId == request.RegionQuery.ManagerId.Value);

                int totalRecords = await query.CountAsync (cancellationToken);
                request.RegionQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.RegionQuery.Page - 1) * request.RegionQuery.PageSize)
                    .Take (request.RegionQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (r => _mapper.Map<RegionModel> (r)).ToList ( );

                var pagedResult = new PagedResult<RegionModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<RegionModel>>.Success (pagedResult);
            }
        }
    }
}
