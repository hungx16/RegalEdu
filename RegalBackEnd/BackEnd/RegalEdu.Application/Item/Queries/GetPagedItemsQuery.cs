using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Queries
{
    public class ItemQuery
    {
        public string? CodeContains { get; set; }
        public string? NameContains { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedItemsQuery : IRequest<Result<PagedResult<ItemModel>>>
    {
        public required ItemQuery Query { get; set; }
    }

    public class Handler_GetPaged : IRequestHandler<GetPagedItemsQuery, Result<PagedResult<ItemModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _paging;
        private readonly IMapper _mapper;

        public Handler_GetPaged(IRegalEducationDbContext context, PagingOptions paging, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _paging = paging ?? throw new ArgumentNullException (nameof (paging));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<ItemModel>>> Handle(GetPagedItemsQuery request, CancellationToken cancellationToken)
        {
            var q = _context.Items.AsNoTracking ( );

            if (!string.IsNullOrWhiteSpace (request.Query.CodeContains))
                q = q.Where (x => x.ItemCode.Contains (request.Query.CodeContains));

            if (!string.IsNullOrWhiteSpace (request.Query.NameContains))
                q = q.Where (x => x.ItemName.Contains (request.Query.NameContains));

            if (request.Query.MinPrice.HasValue) q = q.Where (x => x.Price >= request.Query.MinPrice.Value);
            if (request.Query.MaxPrice.HasValue) q = q.Where (x => x.Price <= request.Query.MaxPrice.Value);
            if (request.Query.MinQuantity.HasValue) q = q.Where (x => x.Quantity >= request.Query.MinQuantity.Value);
            if (request.Query.MaxQuantity.HasValue) q = q.Where (x => x.Quantity <= request.Query.MaxQuantity.Value);

            int total = await q.CountAsync (cancellationToken);
            request.Query.PageSize = _paging.DefaultPageSize;

            var items = await q.OrderByDescending (x => x.CreatedAt)
                .Skip ((request.Query.Page - 1) * request.Query.PageSize)
                .Take (request.Query.PageSize)
                .ToListAsync (cancellationToken);

            var models = items.Select (_mapper.Map<ItemModel>).ToList ( );

            return Result<PagedResult<ItemModel>>.Success (new PagedResult<ItemModel>
            {
                Items = models,
                Total = total
            });
        }
    }
}
