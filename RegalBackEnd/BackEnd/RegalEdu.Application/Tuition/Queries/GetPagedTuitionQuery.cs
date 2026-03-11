using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Queries
{
    public class TuitionQuery
    {
        public string? TuitionName { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? ClassTypeId { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedTuitionsQuery : IRequest<Result<PagedResult<TuitionModel>>>
    {
        public TuitionQuery? TuitionQuery { get; set; }
    }

    public class GetPagedTuitionsQueryHandler : IRequestHandler<GetPagedTuitionsQuery, Result<PagedResult<TuitionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;

        public GetPagedTuitionsQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<TuitionModel>>> Handle(GetPagedTuitionsQuery request, CancellationToken cancellationToken)
        {
            if (request.TuitionQuery == null)
                throw new ArgumentNullException (nameof (request.TuitionQuery));

            var query = _context.Tuition.Include (t => t.Course).Include (t => t.ClassType).AsNoTracking ( );

            if (!string.IsNullOrWhiteSpace (request.TuitionQuery.TuitionName))
            {
                query = query.Where (t => t.TuitionName.Contains (request.TuitionQuery.TuitionName));
            }
            if (request.TuitionQuery.CourseId.HasValue)
            {
                query = query.Where (t => t.CourseId == request.TuitionQuery.CourseId.Value);
            }
            if (request.TuitionQuery.ClassTypeId.HasValue)
            {
                query = query.Where (t => t.ClassTypeId == request.TuitionQuery.ClassTypeId.Value);
            }


            int totalRecords = await query.CountAsync (cancellationToken);
            request.TuitionQuery.PageSize = _pagingOptions.DefaultPageSize;

            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.TuitionQuery.Page - 1) * request.TuitionQuery.PageSize)
                .Take (request.TuitionQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (d => _mapper.Map<TuitionModel> (d)).ToList ( );

            var pagedResult = new PagedResult<TuitionModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<TuitionModel>>.Success (pagedResult);
        }
    }
}
