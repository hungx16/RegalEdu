using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Queries
{
    public class CouponIssueQuery
    {
        public Guid? CouponTypeId { get; set; }
        public IssueType? IssueType { get; set; }
        public DateTime? FromIssueDate { get; set; }
        public DateTime? ToIssueDate { get; set; }
        public int? MinQuantity { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedCouponIssuesQuery : IRequest<Result<PagedResult<CouponIssueModel>>>
    {
        public required CouponIssueQuery CouponIssueQuery { get; set; }
    }

    public class GetPagedCouponIssuesQueryHandler : IRequestHandler<GetPagedCouponIssuesQuery, Result<PagedResult<CouponIssueModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _paging;

        public GetPagedCouponIssuesQueryHandler(IRegalEducationDbContext context, PagingOptions paging)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _paging = paging ?? throw new ArgumentNullException(nameof(paging));
        }

        public async Task<Result<PagedResult<CouponIssueModel>>> Handle(GetPagedCouponIssuesQuery request, CancellationToken cancellationToken)
        {
            var q = request.CouponIssueQuery ?? throw new ArgumentNullException(nameof(request.CouponIssueQuery));

            var query = _context.CouponIssues.AsNoTracking().Where(x => !x.IsDeleted);

            if (q.CouponTypeId.HasValue)
                query = query.Where(x => x.CouponTypeId == q.CouponTypeId);

            if (q.IssueType.HasValue)
                query = query.Where(x => x.IssueType==q.IssueType);

            if (q.FromIssueDate.HasValue)
                query = query.Where(x => x.IssueDate >= q.FromIssueDate);

            if (q.ToIssueDate.HasValue)
                query = query.Where(x => x.IssueDate <= q.ToIssueDate);

            if (q.MinQuantity.HasValue)
                query = query.Where(x => x.Quantity >= q.MinQuantity);

            int total = await query.CountAsync(cancellationToken);
            q.PageSize = q.PageSize > 0 ? q.PageSize : _paging.DefaultPageSize;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(x => new CouponIssueModel
                {
                    Id = x.Id,
                    CouponTypeId = x.CouponTypeId,
                    IssueType = x.IssueType,
                    Quantity = x.Quantity,
                    IssueDate = x.IssueDate,
                    CreatedAt = x.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            var result = new PagedResult<CouponIssueModel> { Items = items, Total = total };
            return Result<PagedResult<CouponIssueModel>>.Success(result);
        }
    }
}
