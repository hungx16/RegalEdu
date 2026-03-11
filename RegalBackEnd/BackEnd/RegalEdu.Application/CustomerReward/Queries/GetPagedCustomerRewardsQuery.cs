using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CustomerReward.Queries
{
    public class CustomerRewardQuery
    {
        public Guid? LuckyDrawId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? RegionId { get; set; }
        public int? ReceiveStatus { get; set; }
        public int? AcceptanceStatus { get; set; }
        public string? PhoneOrName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedCustomerRewardsQuery : IRequest<Result<PagedResult<CustomerRewardModel>>>
    {
        public CustomerRewardQuery? Query { get; set; }

        public class GetPagedCustomerRewardsQueryHandler : IRequestHandler<GetPagedCustomerRewardsQuery, Result<PagedResult<CustomerRewardModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly PagingOptions _pagingOptions;

            public GetPagedCustomerRewardsQueryHandler(IRegalEducationDbContext context, IMapper mapper, PagingOptions pagingOptions)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
            }

            public async Task<Result<PagedResult<CustomerRewardModel>>> Handle(GetPagedCustomerRewardsQuery request, CancellationToken cancellationToken)
            {
                if (request.Query == null) throw new ArgumentNullException(nameof(request.Query));
                var q = _context.SetEntity<RegalEdu.Domain.Entities.CustomerReward>().AsNoTracking();
                if (request.Query.LuckyDrawId.HasValue) q = q.Where(x => x.LuckyDrawId == request.Query.LuckyDrawId.Value);
                if (request.Query.CompanyId.HasValue) q = q.Where(x => x.CompanyId == request.Query.CompanyId.Value);
                if (request.Query.RegionId.HasValue) q = q.Where(x => x.RegionId == request.Query.RegionId.Value);
                if (request.Query.ReceiveStatus.HasValue) q = q.Where(x => x.ReceiveStatus == request.Query.ReceiveStatus.Value);
                if (request.Query.AcceptanceStatus.HasValue) q = q.Where(x => x.AcceptanceStatus == request.Query.AcceptanceStatus.Value);
                if (!string.IsNullOrWhiteSpace(request.Query.PhoneOrName)) q = q.Where(x => x.Phone.Contains(request.Query.PhoneOrName) || x.FullName.Contains(request.Query.PhoneOrName));

                var total = await q.CountAsync(cancellationToken);
                request.Query.PageSize = _pagingOptions.DefaultPageSize;
                var list = await q.OrderByDescending(x => x.WonDate)
                    .Skip((request.Query.Page - 1) * request.Query.PageSize)
                    .Take(request.Query.PageSize)
                    .ToListAsync(cancellationToken);

                var models = list.Select(x => _mapper.Map<CustomerRewardModel>(x)).ToList();
                var paged = new PagedResult<CustomerRewardModel> { Items = models, Total = total };
                return Result<PagedResult<CustomerRewardModel>>.Success(paged);
            }
        }
    }
}
