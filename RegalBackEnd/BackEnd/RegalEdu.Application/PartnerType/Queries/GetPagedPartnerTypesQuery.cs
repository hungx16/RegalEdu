using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PartnerType.Queries
{
    public class PartnerTypeQuery
    {
        public string? CodeContains { get; set; }
        public string? NameContains { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedPartnerTypesQuery : IRequest<Result<PagedResult<PartnerTypeModel>>>
    {
        public required PartnerTypeQuery Query { get; set; }
    }

    public class Handler_GetPaged : IRequestHandler<GetPagedPartnerTypesQuery, Result<PagedResult<PartnerTypeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _paging;
        private readonly IMapper _mapper;

        public Handler_GetPaged(IRegalEducationDbContext context, PagingOptions paging, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _paging = paging ?? throw new ArgumentNullException(nameof(paging));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<PagedResult<PartnerTypeModel>>> Handle(GetPagedPartnerTypesQuery request, CancellationToken cancellationToken)
        {
            var q = _context.PartnerTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Query.CodeContains))
                q = q.Where(x => x.PartnerTypeCode.Contains(request.Query.CodeContains));

            if (!string.IsNullOrWhiteSpace(request.Query.NameContains))
                q = q.Where(x => x.PartnerTypeName.Contains(request.Query.NameContains));

            int total = await q.CountAsync(cancellationToken);
            request.Query.PageSize = _paging.DefaultPageSize;

            var items = await q.OrderByDescending(x => x.CreatedAt)
                .Skip((request.Query.Page - 1) * request.Query.PageSize)
                .Take(request.Query.PageSize)
                .ToListAsync(cancellationToken);

            var models = items.Select(_mapper.Map<PartnerTypeModel>).ToList();

            return Result<PagedResult<PartnerTypeModel>>.Success(new PagedResult<PartnerTypeModel>
            {
                Items = models,
                Total = total
            });
        }
    }
}