using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Gift.Queries
{
    public class GiftQuery
    {
        public string? Name { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedGiftsQuery : IRequest<Result<PagedResult<GiftModel>>>
    {
        public GiftQuery? GiftQuery { get; set; }
    
    }

    public class GetPagedGiftsQueryHandler : IRequestHandler<GetPagedGiftsQuery, Result<PagedResult<GiftModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;

        public GetPagedGiftsQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
        }

        public async Task<Result<PagedResult<GiftModel>>> Handle(GetPagedGiftsQuery request, CancellationToken cancellationToken)
        {
            if (request.GiftQuery == null)
                throw new ArgumentNullException(nameof(request.GiftQuery));

            var query = _context.Gift.AsNoTracking().Where(g => !g.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.GiftQuery.Name))
                query = query.Where(g => g.Name!.Contains(request.GiftQuery.Name));

            if (request.GiftQuery.MinPrice.HasValue)
                query = query.Where(g => g.Prices >= request.GiftQuery.MinPrice.Value);

            if (request.GiftQuery.MaxPrice.HasValue)
                query = query.Where(g => g.Prices <= request.GiftQuery.MaxPrice.Value);

            int totalRecords = await query.CountAsync(cancellationToken);
            request.GiftQuery.PageSize = _pagingOptions.DefaultPageSize;

            var paged = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.GiftQuery.Page - 1) * request.GiftQuery.PageSize)
                .Take(request.GiftQuery.PageSize)
                .ToListAsync(cancellationToken);

            var result = paged.Select(g => new GiftModel
            {
                Id = g.Id,
                Name = g.Name,
                Prices = g.Prices,
                Description = g.Description,
                Status = g.Status,
                CreatedAt = g.CreatedAt
            }).ToList();

            var pagedResult = new PagedResult<GiftModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<GiftModel>>.Success(pagedResult);
        }
    }
}
