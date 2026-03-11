using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Queries
{
    public class CouponTypeQuery
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedCouponTypesQuery : IRequest<Result<PagedResult<CouponTypeModel>>>
    {
        public CouponTypeQuery? CouponTypeQuery { get; set; }
    }

    public class GetPagedCouponTypesQueryHandler : IRequestHandler<GetPagedCouponTypesQuery, Result<PagedResult<CouponTypeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;

        public GetPagedCouponTypesQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
        }

        public async Task<Result<PagedResult<CouponTypeModel>>> Handle(GetPagedCouponTypesQuery request, CancellationToken cancellationToken)
        {
            if (request.CouponTypeQuery == null)
                throw new ArgumentNullException(nameof(request.CouponTypeQuery));

            var query = _context.CouponType.AsNoTracking().Where(ct => !ct.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.CouponTypeQuery.Name))
                query = query.Where(ct => ct.Name!.Contains(request.CouponTypeQuery.Name));

            if (!string.IsNullOrWhiteSpace(request.CouponTypeQuery.Code))
                query = query.Where(ct => ct.Code!.Contains(request.CouponTypeQuery.Code));

            int totalRecords = await query.CountAsync(cancellationToken);
            request.CouponTypeQuery.PageSize = _pagingOptions.DefaultPageSize;

            var paged = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.CouponTypeQuery.Page - 1) * request.CouponTypeQuery.PageSize)
                .Take(request.CouponTypeQuery.PageSize)
                .ToListAsync(cancellationToken);

            var result = paged.Select(ct => new CouponTypeModel
            {
                Id = ct.Id,
                Name = ct.Name,
                Code = ct.Code,
                Description = ct.Description,
                Status = ct.Status,
                CreatedAt = ct.CreatedAt
            }).ToList();

            var pagedResult = new PagedResult<CouponTypeModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<CouponTypeModel>>.Success(pagedResult);
        }
    }
}
