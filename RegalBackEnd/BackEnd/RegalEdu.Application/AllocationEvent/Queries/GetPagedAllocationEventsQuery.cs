using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class AllocationEventQuery
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedAllocationEventsQuery : IRequest<Result<PagedResult<AllocationEventModel>>>
    {
        public AllocationEventQuery? AllocationEventQuery { get; set; }
    }

    public class GetPagedAllocationEventsQueryHandler
        : IRequestHandler<GetPagedAllocationEventsQuery, Result<PagedResult<AllocationEventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;

        public GetPagedAllocationEventsQueryHandler(
            IRegalEducationDbContext context,
            PagingOptions pagingOptions,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<PagedResult<AllocationEventModel>>> Handle(
            GetPagedAllocationEventsQuery request,
            CancellationToken cancellationToken)
        {
            if (request.AllocationEventQuery == null)
                throw new ArgumentNullException(nameof(request.AllocationEventQuery));

            var query = _context.AllocationEvents
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            // --- Bộ lọc ---
            if (request.AllocationEventQuery.Year.HasValue)
            {
                query = query.Where(x => x.AllocationYear == request.AllocationEventQuery.Year.Value);
            }

            if (request.AllocationEventQuery.Month.HasValue)
            {
                query = query.Where(x => x.AllocationMonth == request.AllocationEventQuery.Month.Value);
            }

            if (request.AllocationEventQuery.Status.HasValue)
            {
                query = query.Where(x => (byte) x.AllocationEventStatus == request.AllocationEventQuery.Status.Value);
            }

            // --- Phân trang ---
            int totalRecords = await query.CountAsync(cancellationToken);
            request.AllocationEventQuery.PageSize = _pagingOptions.DefaultPageSize;

            var paged = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.AllocationEventQuery.Page - 1) * request.AllocationEventQuery.PageSize)
                .Take(request.AllocationEventQuery.PageSize)
                .ToListAsync(cancellationToken);

            var result = paged.Select(x => _mapper.Map<AllocationEventModel>(x)).ToList();

            var pagedResult = new PagedResult<AllocationEventModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<AllocationEventModel>>.Success(pagedResult);
        }
    }
}
