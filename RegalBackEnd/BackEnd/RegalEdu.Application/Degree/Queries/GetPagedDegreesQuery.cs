using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Degree.Queries
{
    public class DegreeQuery
    {
        public string? DegreeName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedDegreesQuery : IRequest<Result<PagedResult<DegreeModel>>>
    {
        public DegreeQuery? DegreeQuery { get; set; }

        public class GetPagedDegreesQueryHandler : IRequestHandler<GetPagedDegreesQuery, Result<PagedResult<DegreeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedDegreesQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<DegreeModel>>> Handle(GetPagedDegreesQuery request, CancellationToken cancellationToken)
            {
                if (request.DegreeQuery == null)
                    throw new ArgumentNullException (nameof (request.DegreeQuery));

                var query = _context.Degrees.AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.DegreeQuery.DegreeName))
                    query = query.Where (x => x.DegreeName.Contains (request.DegreeQuery.DegreeName));

                int totalRecords = await query.CountAsync (cancellationToken);
                request.DegreeQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.DegreeQuery.Page - 1) * request.DegreeQuery.PageSize)
                    .Take (request.DegreeQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<DegreeModel> (x)).ToList ( );

                var pagedResult = new PagedResult<DegreeModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<DegreeModel>>.Success (pagedResult);
            }
        }
    }
}
