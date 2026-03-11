using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkBoardTeacher.Queries
{
    public class WorkBoardTeacherQuery
    {
        public Guid? TeacherId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
        public bool? IsConfirmed { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedWorkBoardTeachersQuery : IRequest<Result<PagedResult<WorkBoardTeacherModel>>>
    {
        public WorkBoardTeacherQuery? WorkBoardTeacherQuery { get; set; }

        public class GetPagedWorkBoardTeachersQueryHandler : IRequestHandler<GetPagedWorkBoardTeachersQuery, Result<PagedResult<WorkBoardTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedWorkBoardTeachersQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<PagedResult<WorkBoardTeacherModel>>> Handle(GetPagedWorkBoardTeachersQuery request, CancellationToken cancellationToken)
            {
                if (request.WorkBoardTeacherQuery == null)
                    throw new ArgumentNullException(nameof(request.WorkBoardTeacherQuery));

                var query = _context.WorkBoardTeachers
                    .Include(wbt => wbt.Teacher)
                    .AsNoTracking();

                if (request.WorkBoardTeacherQuery.TeacherId.HasValue)
                    query = query.Where(wbt => wbt.TeacherId == request.WorkBoardTeacherQuery.TeacherId.Value);

                if (request.WorkBoardTeacherQuery.FromDate.HasValue)
                    query = query.Where(wbt => wbt.Date >= request.WorkBoardTeacherQuery.FromDate.Value);

                if (request.WorkBoardTeacherQuery.ToDate.HasValue)
                    query = query.Where(wbt => wbt.Date <= request.WorkBoardTeacherQuery.ToDate.Value);

                if (request.WorkBoardTeacherQuery.Status.HasValue)
                    query = query.Where(wbt => wbt.Status == request.WorkBoardTeacherQuery.Status.Value);

                if (request.WorkBoardTeacherQuery.IsConfirmed.HasValue)
                    query = query.Where(wbt => wbt.IsConfirmed == request.WorkBoardTeacherQuery.IsConfirmed.Value);

                int totalRecords = await query.CountAsync(cancellationToken);
                request.WorkBoardTeacherQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.CreatedAt)
                    .Skip((request.WorkBoardTeacherQuery.Page - 1) * request.WorkBoardTeacherQuery.PageSize)
                    .Take(request.WorkBoardTeacherQuery.PageSize)
                    .ToListAsync(cancellationToken);

                var result = paged.Select(wbt => _mapper.Map<WorkBoardTeacherModel>(wbt)).ToList();

                var pagedResult = new PagedResult<WorkBoardTeacherModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<WorkBoardTeacherModel>>.Success(pagedResult);
            }
        }
    }
}