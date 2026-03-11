using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LuckyDraw.Queries
{
    public class LuckyDrawQuery
    {
        public string? Name { get; set; }
        public string? Branch { get; set; }
        public string? Region { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedLuckyDrawsQuery : IRequest<Result<PagedResult<LuckyDrawModel>>>
    {
        public LuckyDrawQuery? Query { get; set; }

        public class GetPagedLuckyDrawsQueryHandler : IRequestHandler<GetPagedLuckyDrawsQuery, Result<PagedResult<LuckyDrawModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly PagingOptions _pagingOptions;

            public GetPagedLuckyDrawsQueryHandler(IRegalEducationDbContext context, IMapper mapper, PagingOptions pagingOptions)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
            }

            public async Task<Result<PagedResult<LuckyDrawModel>>> Handle(GetPagedLuckyDrawsQuery request, CancellationToken cancellationToken)
            {
                var q = _context.SetEntity<RegalEdu.Domain.Entities.LuckyDraw>().AsNoTracking();
                if (request.Query == null) throw new ArgumentNullException(nameof(request.Query));
                if (!string.IsNullOrWhiteSpace(request.Query.Name)) q = q.Where(x => x.Name.Contains(request.Query.Name));
                if (!string.IsNullOrWhiteSpace(request.Query.Branch)) q = q.Where(x => x.Branch == request.Query.Branch);
                if (!string.IsNullOrWhiteSpace(request.Query.Region)) q = q.Where(x => x.Region == request.Query.Region);

                var total = await q.CountAsync(cancellationToken);
                request.Query.PageSize = _pagingOptions.DefaultPageSize;
                var list = await q.OrderByDescending(x => x.CreatedAt)
                    .Skip((request.Query.Page - 1) * request.Query.PageSize)
                    .Take(request.Query.PageSize)
                    .ToListAsync(cancellationToken);

                var models = list.Select(x => _mapper.Map<LuckyDrawModel>(x)).ToList();
                var paged = new PagedResult<LuckyDrawModel> { Items = models, Total = total };
                return Result<PagedResult<LuckyDrawModel>>.Success(paged);
            }
        }
    }
}
