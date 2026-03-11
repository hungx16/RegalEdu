using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LectureType.Queries
{
    public class LectureTypeQuery
    {
        public string? LectureName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedLectureTypesQuery : IRequest<Result<PagedResult<LectureTypeModel>>>
    {
        public LectureTypeQuery? LectureTypeQuery { get; set; }

        public class GetPagedLectureTypesQueryHandler : IRequestHandler<GetPagedLectureTypesQuery, Result<PagedResult<LectureTypeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedLectureTypesQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<LectureTypeModel>>> Handle(GetPagedLectureTypesQuery request, CancellationToken cancellationToken)
            {
                if (request.LectureTypeQuery == null)
                    throw new ArgumentNullException (nameof (request.LectureTypeQuery));

                var query = _context.LectureTypes.AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.LectureTypeQuery.LectureName))
                    query = query.Where (x => x.LectureName.Contains (request.LectureTypeQuery.LectureName));

                int totalRecords = await query.CountAsync (cancellationToken);
                request.LectureTypeQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.LectureTypeQuery.Page - 1) * request.LectureTypeQuery.PageSize)
                    .Take (request.LectureTypeQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<LectureTypeModel> (x)).ToList ( );

                var pagedResult = new PagedResult<LectureTypeModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<LectureTypeModel>>.Success (pagedResult);
            }
        }
    }
}
