using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Course.Queries
{
    public class CourseQuery
    {
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public byte? CourseStatus { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
        public StatusType? Status { get; set; }
    }

    public class GetPagedCoursesQuery : IRequest<Result<PagedResult<CourseModel>>>
    {
        public CourseQuery? CourseQuery { get; set; }
    }

    public class GetPagedCoursesQueryHandler : IRequestHandler<GetPagedCoursesQuery, Result<PagedResult<CourseModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;
        public GetPagedCoursesQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<CourseModel>>> Handle(GetPagedCoursesQuery request, CancellationToken cancellationToken)
        {
            if (request.CourseQuery == null)
            {
                throw new ArgumentNullException (nameof (request.CourseQuery));
            }
            var query = _context.Courses.Include (c => c.LearningRoadMap).Include (c => c.DetailRegisterStudies).Include(c => c.Tuitions).AsNoTracking ( );


            if (!string.IsNullOrWhiteSpace (request.CourseQuery.CourseCode))
            {
                query = query.Where (d => d.CourseCode.Contains (request.CourseQuery.CourseCode));
            }
            if (!string.IsNullOrWhiteSpace (request.CourseQuery.CourseName))
            {
                query = query.Where (d => d.CourseName.Contains (request.CourseQuery.CourseName));
            }
            if (request.CourseQuery.CourseStatus.HasValue)
            {
                query = query.Where (d => d.Status == (StatusType)request.CourseQuery.Status.Value);

            }

            int totalRecords = await query.CountAsync (cancellationToken);
            request.CourseQuery.PageSize = _pagingOptions.DefaultPageSize;
            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.CourseQuery.Page - 1) * request.CourseQuery.PageSize)
                .Take (request.CourseQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (d => _mapper.Map<CourseModel> (d)).ToList ( );

            var pagedResult = new PagedResult<CourseModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<CourseModel>>.Success (pagedResult);

        }
    }
}