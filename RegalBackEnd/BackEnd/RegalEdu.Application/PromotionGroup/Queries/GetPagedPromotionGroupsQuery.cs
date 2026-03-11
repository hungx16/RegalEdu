using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Queries
{
    public class PromotionGroupQuery
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedPromotionGroupsQuery : IRequest<Result<PagedResult<PromotionGroupModel>>>
    {
        public PromotionGroupQuery? PromotionGroupQuery { get; set; }
    }

    public class GetPagedPromotionGroupsQueryHandler : IRequestHandler<GetPagedPromotionGroupsQuery, Result<PagedResult<PromotionGroupModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;

        public GetPagedPromotionGroupsQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
        }

        public async Task<Result<PagedResult<PromotionGroupModel>>> Handle(GetPagedPromotionGroupsQuery request, CancellationToken cancellationToken)
        {
            if (request.PromotionGroupQuery == null)
                throw new ArgumentNullException(nameof(request.PromotionGroupQuery));

            var query = _context.PromotionGroup.AsNoTracking().Where(pg => !pg.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.PromotionGroupQuery.Name))
            {
                query = query.Where(pg => pg.Name!.Contains(request.PromotionGroupQuery.Name));
            }

            if (!string.IsNullOrWhiteSpace(request.PromotionGroupQuery.Description))
            {
                query = query.Where(pg => pg.Description!.Contains(request.PromotionGroupQuery.Description));
            }

            int totalRecords = await query.CountAsync(cancellationToken);
            request.PromotionGroupQuery.PageSize = _pagingOptions.DefaultPageSize;

            var paged = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PromotionGroupQuery.Page - 1) * request.PromotionGroupQuery.PageSize)
                .Take(request.PromotionGroupQuery.PageSize)
                .ToListAsync(cancellationToken);

            var result = paged.Select(pg => new PromotionGroupModel
            {
                Id = pg.Id,
                Name = pg.Name,
                Description = pg.Description,
                Status = pg.Status,
                CreatedAt = pg.CreatedAt
            }).ToList();

            var pagedResult = new PagedResult<PromotionGroupModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<PromotionGroupModel>>.Success(pagedResult);
        }
    }
}
