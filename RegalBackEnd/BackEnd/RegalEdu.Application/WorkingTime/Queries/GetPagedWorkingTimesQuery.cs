using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Queries
{
    public class WorkingTimeQuery
    {
        public string? Name { get; set; }
        public byte? DayOfWeek { get; set; }
        public Guid? RegionId { get; set; }
        public bool? IsWorkingDay { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedWorkingTimesQuery : IRequest<Result<PagedResult<WorkingTimeModel>>>
    {
        public WorkingTimeQuery? WorkingTimeQuery { get; set; }

        public class GetPagedWorkingTimesQueryHandler : IRequestHandler<GetPagedWorkingTimesQuery, Result<PagedResult<WorkingTimeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedWorkingTimesQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<WorkingTimeModel>>> Handle(GetPagedWorkingTimesQuery request, CancellationToken cancellationToken)
            {
                if (request.WorkingTimeQuery == null)
                    throw new ArgumentNullException (nameof (request.WorkingTimeQuery));

                var query = _context.WorkingTimes
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.WorkingTimeQuery.Name))
                    query = query.Where (x => x.Name.Contains (request.WorkingTimeQuery.Name));
                if (request.WorkingTimeQuery.DayOfWeek.HasValue)
                    query = query.Where (x => x.DayOfWeek == request.WorkingTimeQuery.DayOfWeek.Value);

                if (request.WorkingTimeQuery.IsWorkingDay.HasValue)
                    query = query.Where (x => x.IsWorkingDay == request.WorkingTimeQuery.IsWorkingDay.Value);

                int totalRecords = await query.CountAsync (cancellationToken);
                request.WorkingTimeQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.WorkingTimeQuery.Page - 1) * request.WorkingTimeQuery.PageSize)
                    .Take (request.WorkingTimeQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<WorkingTimeModel> (x)).ToList ( );

                var pagedResult = new PagedResult<WorkingTimeModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<WorkingTimeModel>>.Success (pagedResult);
            }
        }
    }
}
