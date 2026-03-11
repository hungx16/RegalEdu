using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Reward.Queries
{
    public class RewardQuery
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedRewardsQuery : IRequest<Result<PagedResult<RewardModel>>>
    {
        public RewardQuery? Query { get; set; }

        public class GetPagedRewardsQueryHandler : IRequestHandler<GetPagedRewardsQuery, Result<PagedResult<RewardModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly PagingOptions _pagingOptions;

            public GetPagedRewardsQueryHandler(IRegalEducationDbContext context, IMapper mapper, PagingOptions pagingOptions)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
            }

            public async Task<Result<PagedResult<RewardModel>>> Handle(GetPagedRewardsQuery request, CancellationToken cancellationToken)
            {
                var q = _context.SetEntity<RegalEdu.Domain.Entities.Reward>().AsNoTracking();
                if (request.Query == null) throw new ArgumentNullException(nameof(request.Query));
                if (!string.IsNullOrWhiteSpace(request.Query.Name)) q = q.Where(x => x.Name.Contains(request.Query.Name));
                if (!string.IsNullOrWhiteSpace(request.Query.Type)) q = q.Where(x => x.Type == request.Query.Type);

                var total = await q.CountAsync(cancellationToken);
                request.Query.PageSize = _pagingOptions.DefaultPageSize;
                var list = await q.OrderByDescending(x => x.CreatedAt)
                    .Skip((request.Query.Page - 1) * request.Query.PageSize)
                    .Take(request.Query.PageSize)
                    .ToListAsync(cancellationToken);

                var models = list.Select(x => _mapper.Map<RewardModel>(x)).ToList();
                var paged = new PagedResult<RewardModel> { Items = models, Total = total };
                return Result<PagedResult<RewardModel>>.Success(paged);
            }
        }
    }
}
