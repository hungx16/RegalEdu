using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Queries
{
    public class LearningRoadMapQuery
    {
        public string? LearningRoadMapCode { get; set; }
        public string? LearningRoadMapName { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedLearningRoadMapsQuery : IRequest<Result<PagedResult<LearningRoadMapModel>>>
    {
        public LearningRoadMapQuery? LearningRoadMapQuery { get; set; }
    }

    public class GetPagedLearningRoadMapsQueryHandler : IRequestHandler<GetPagedLearningRoadMapsQuery, Result<PagedResult<LearningRoadMapModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _Mapper;
        public GetPagedLearningRoadMapsQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions, IMapper Mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper));
        }

        public async Task<Result<PagedResult<LearningRoadMapModel>>> Handle(GetPagedLearningRoadMapsQuery request, CancellationToken cancellationToken)
        {
            if (request.LearningRoadMapQuery == null)
            {
                throw new ArgumentNullException (nameof (request.LearningRoadMapQuery));
            }
            var query = _context.LearningRoadMaps.Include(c => c.AgeGroup).Include(c => c.Courses).AsNoTracking ( );

            if (!string.IsNullOrWhiteSpace (request.LearningRoadMapQuery.LearningRoadMapCode))
            {
                query = query.Where (d => d.LearningRoadMapCode.Contains (request.LearningRoadMapQuery.LearningRoadMapCode));
            }
            if (!string.IsNullOrWhiteSpace (request.LearningRoadMapQuery.LearningRoadMapName))
            {
                query = query.Where (d => d.LearningRoadMapName.Contains (request.LearningRoadMapQuery.LearningRoadMapName));
            }


            int totalRecords = await query.CountAsync (cancellationToken);
            request.LearningRoadMapQuery.PageSize = _pagingOptions.DefaultPageSize;
            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.LearningRoadMapQuery.Page - 1) * request.LearningRoadMapQuery.PageSize)
                .Take (request.LearningRoadMapQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (d => _Mapper.Map<LearningRoadMapModel> (d)).ToList ( );

            var pagedResult = new PagedResult<LearningRoadMapModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<LearningRoadMapModel>>.Success (pagedResult);

        }
    }
}