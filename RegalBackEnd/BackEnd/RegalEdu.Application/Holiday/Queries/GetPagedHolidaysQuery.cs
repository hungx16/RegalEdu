using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Queries
{
    public class HolidayQuery
    {
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public Guid? RegionId { get; set; }
        //public Guid? CategoryId { get; set; }
        public byte? Frequency { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedHolidaysQuery : IRequest<Result<PagedResult<HolidayModel>>>
    {
        public HolidayQuery? HolidayQuery { get; set; }

        public class GetPagedHolidaysQueryHandler : IRequestHandler<GetPagedHolidaysQuery, Result<PagedResult<HolidayModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedHolidaysQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<HolidayModel>>> Handle(GetPagedHolidaysQuery request, CancellationToken cancellationToken)
            {
                if (request.HolidayQuery == null)
                    throw new ArgumentNullException (nameof (request.HolidayQuery));

                var query = _context.Holidays
                    .Include (x => x.Category)
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.HolidayQuery.Name))
                    query = query.Where (x => x.Name.Contains (request.HolidayQuery.Name));
                if (request.HolidayQuery.Date.HasValue)
                    query = query.Where (x => x.Date == request.HolidayQuery.Date.Value);

                //if (request.HolidayQuery.CategoryId.HasValue)
                //    query = query.Where (x => x.CategoryId == request.HolidayQuery.CategoryId.Value);
                if (request.HolidayQuery.Frequency.HasValue)
                    query = query.Where (x => x.Frequency == request.HolidayQuery.Frequency.Value);

                int totalRecords = await query.CountAsync (cancellationToken);
                request.HolidayQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.Date)
                    .Skip ((request.HolidayQuery.Page - 1) * request.HolidayQuery.PageSize)
                    .Take (request.HolidayQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<HolidayModel> (x)).ToList ( );

                var pagedResult = new PagedResult<HolidayModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<HolidayModel>>.Success (pagedResult);
            }
        }
    }
}
