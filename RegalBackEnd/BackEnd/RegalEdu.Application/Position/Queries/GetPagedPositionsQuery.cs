using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Queries
{
    public class PositionQuery
    {
        public string? PositionCode { get; set; }
        public string? PositionName { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedPositionsQuery : IRequest<Result<PagedResult<PositionModel>>>
    {
        public PositionQuery? PositionQuery { get; set; }
    }

    public class GetPagedPositionsQueryHandler : IRequestHandler<GetPagedPositionsQuery, Result<PagedResult<PositionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;

        public GetPagedPositionsQueryHandler(
            IRegalEducationDbContext context,
            PagingOptions pagingOptions,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<PositionModel>>> Handle(GetPagedPositionsQuery request, CancellationToken cancellationToken)
        {
            if (request.PositionQuery == null)
                throw new ArgumentNullException (nameof (request.PositionQuery));

            var query = _context.Positions.AsNoTracking ( );

            if (!string.IsNullOrWhiteSpace (request.PositionQuery.PositionCode))
                query = query.Where (d => d.PositionCode.Contains (request.PositionQuery.PositionCode));
            if (!string.IsNullOrWhiteSpace (request.PositionQuery.PositionName))
                query = query.Where (d => d.PositionName.Contains (request.PositionQuery.PositionName));


            int totalRecords = await query.CountAsync (cancellationToken);
            request.PositionQuery.PageSize = _pagingOptions.DefaultPageSize;
            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.PositionQuery.Page - 1) * request.PositionQuery.PageSize)
                .Take (request.PositionQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (d => _mapper.Map<PositionModel> (d)).ToList ( );

            var pagedResult = new PagedResult<PositionModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<PositionModel>>.Success (pagedResult);
        }
    }
}
