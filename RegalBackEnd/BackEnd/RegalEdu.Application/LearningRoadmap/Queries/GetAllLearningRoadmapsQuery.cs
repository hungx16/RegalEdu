using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Queries
{
    public class GetAllLearningRoadMapsQuery : IRequest<Result<List<LearningRoadMapModel>>> { }

    public class GetAllLearningRoadMapsQueryHandler : IRequestHandler<GetAllLearningRoadMapsQuery, Result<List<LearningRoadMapModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _Mapper;

        public GetAllLearningRoadMapsQueryHandler(IRegalEducationDbContext context, IMapper Mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper)); ;
        }

        public async Task<Result<List<LearningRoadMapModel>>> Handle(GetAllLearningRoadMapsQuery request, CancellationToken cancellationToken)
        {
            var learningRoadMaps = await _context.LearningRoadMaps.Include (c => c.AgeGroup).Include (t => t.Images).Include (c => c.Courses).AsNoTracking ( ).ToListAsync (cancellationToken);
            var result = _Mapper.Map<List<LearningRoadMapModel>> (learningRoadMaps);

            return Result<List<LearningRoadMapModel>>.Success (result);
        }
    }
}