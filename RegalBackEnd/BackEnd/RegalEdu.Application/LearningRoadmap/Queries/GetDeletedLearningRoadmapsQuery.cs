using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Queries
{
    public class GetDeletedLearningRoadMapsQuery : IRequest<Result<List<LearningRoadMapModel>>> { }

    public class GetDeletedLearningRoadMapsQueryHandler : IRequestHandler<GetDeletedLearningRoadMapsQuery, Result<List<LearningRoadMapModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _Mapper;

        public GetDeletedLearningRoadMapsQueryHandler(IRegalEducationDbContext context, IMapper Mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper)); ;
        }

        public async Task<Result<List<LearningRoadMapModel>>> Handle(GetDeletedLearningRoadMapsQuery request, CancellationToken cancellationToken)
        {
            var learningRoadMaps = await _context.LearningRoadMaps.IgnoreQueryFilters ( ).Where (d => d.IsDeleted).AsNoTracking ( ).ToListAsync (cancellationToken);
            var result = _Mapper.Map<List<LearningRoadMapModel>> (learningRoadMaps);

            return Result<List<LearningRoadMapModel>>.Success (result);
        }
    }
}