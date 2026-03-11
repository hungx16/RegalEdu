using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.EvaluateTeacher.Queries
{
    public class EvaluateTeacherQuery
    {
        public Guid? TeacherId { get; set; }
        public string? EvaluateName { get; set; }
        public double? MinStar { get; set; }
        public double? MaxStar { get; set; }
        public string? EvaluateType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedEvaluateTeachersQuery : IRequest<Result<PagedResult<EvaluateTeacherModel>>>
    {
        public EvaluateTeacherQuery? EvaluateTeacherQuery { get; set; }

        public class GetPagedEvaluateTeachersQueryHandler : IRequestHandler<GetPagedEvaluateTeachersQuery, Result<PagedResult<EvaluateTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedEvaluateTeachersQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException(nameof(pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<PagedResult<EvaluateTeacherModel>>> Handle(GetPagedEvaluateTeachersQuery request, CancellationToken cancellationToken)
            {
                if (request.EvaluateTeacherQuery == null)
                    throw new ArgumentNullException(nameof(request.EvaluateTeacherQuery));

                var query = _context.EvaluateTeachers
                    .Include(et => et.Teacher)
                    .AsNoTracking();

                if (request.EvaluateTeacherQuery.TeacherId.HasValue)
                    query = query.Where(et => et.TeacherId == request.EvaluateTeacherQuery.TeacherId.Value);

                if (!string.IsNullOrWhiteSpace(request.EvaluateTeacherQuery.EvaluateName))
                    query = query.Where(et => et.EvaluateName != null && et.EvaluateName.Contains(request.EvaluateTeacherQuery.EvaluateName));

                if (request.EvaluateTeacherQuery.MinStar.HasValue)
                    query = query.Where(et => et.StarRating >= request.EvaluateTeacherQuery.MinStar.Value);

                if (request.EvaluateTeacherQuery.MaxStar.HasValue)
                    query = query.Where(et => et.StarRating <= request.EvaluateTeacherQuery.MaxStar.Value);

                if (!string.IsNullOrWhiteSpace(request.EvaluateTeacherQuery.EvaluateType))
                    query = query.Where(et => et.EvaluateType == request.EvaluateTeacherQuery.EvaluateType);

                int totalRecords = await query.CountAsync(cancellationToken);
                request.EvaluateTeacherQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending(x => x.EvaluateDate)
                    .Skip((request.EvaluateTeacherQuery.Page - 1) * request.EvaluateTeacherQuery.PageSize)
                    .Take(request.EvaluateTeacherQuery.PageSize)
                    .ToListAsync(cancellationToken);

                var result = paged.Select(et => _mapper.Map<EvaluateTeacherModel>(et)).ToList();

                var pagedResult = new PagedResult<EvaluateTeacherModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<EvaluateTeacherModel>>.Success(pagedResult);
            }
        }
    }
}
