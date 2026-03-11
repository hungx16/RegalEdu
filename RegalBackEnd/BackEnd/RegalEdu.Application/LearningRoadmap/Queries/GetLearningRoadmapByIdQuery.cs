using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Queries
{
    public class GetLearningRoadMapByIdQuery : IRequest<Result<LearningRoadMapModel>>
    {
        public required string Id { get; set; }

    }

    public class GetLearningRoadMapByIdQueryHandler : IRequestHandler<GetLearningRoadMapByIdQuery, Result<LearningRoadMapModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _Mapper;
        private readonly ILocalizationService _localizer;

        public GetLearningRoadMapByIdQueryHandler(IRegalEducationDbContext context, IMapper Mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _Mapper = Mapper ?? throw new ArgumentNullException(nameof(Mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<LearningRoadMapModel>> Handle(GetLearningRoadMapByIdQuery request, CancellationToken cancellationToken)
        {
            // Chỉ lấy bản ghi chưa bị xoá mềm (IsDeleted == false)
            var learningRoadMap = await _context.LearningRoadMaps
                .Include(c => c.Courses)

                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, cancellationToken);

            if (learningRoadMap == null)
            {
                var msg = _localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.LearningRoadMap], request.Id);
                return Result<LearningRoadMapModel>.Failure(msg);
            }

            var result = _Mapper.Map<LearningRoadMapModel>(learningRoadMap);
            return Result<LearningRoadMapModel>.Success(result);
        }
    }
}