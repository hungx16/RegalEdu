using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Queries
{
    public class GetTuitionByCourseIdAndClassTypeIdQuery : IRequest<Result<List<TuitionModel>>>
    {
        public required string CourseId { get; set; }
        public required string ClassTypeId { get; set; }
    }

    public class GetTuitionByCourseIdAndClassTypeIdQueryHandler : IRequestHandler<GetTuitionByCourseIdAndClassTypeIdQuery, Result<List<TuitionModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetTuitionByCourseIdAndClassTypeIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<List<TuitionModel>>> Handle(GetTuitionByCourseIdAndClassTypeIdQuery request, CancellationToken cancellationToken)
        {
            var tuition = _context.Tuition
                .Include (x => x.Course)
                .Include (x => x.ClassType)
                .AsNoTracking ( );

            if (!string.IsNullOrEmpty (request.CourseId))
            {
                if (Guid.TryParse (request.CourseId, out var courseGuid))
                {
                    tuition = tuition.Where (t => t.CourseId == courseGuid);
                }
                else
                {
                    var msg = _localizer.Format (LocalizationKey.InvalidGuidFormat, _localizer[EntityName.Course], request.CourseId);
                    return Result<List<TuitionModel>>.Failure (msg);
                }
            }

            if (!string.IsNullOrEmpty (request.ClassTypeId))
            {
                if (Guid.TryParse (request.ClassTypeId, out var classTypeGuid))
                {
                    tuition = tuition.Where (t => t.ClassTypeId == classTypeGuid);
                }
                else
                {
                    var msg = _localizer.Format (LocalizationKey.InvalidGuidFormat, _localizer[EntityName.ClassType], request.ClassTypeId);
                    return Result<List<TuitionModel>>.Failure (msg);
                }
            }

            var tuitionEntity = await tuition.ToListAsync (cancellationToken);

            if (tuitionEntity == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.Tuition], $"{request.CourseId}, {request.ClassTypeId}");
                return Result<List<TuitionModel>>.Failure (msg);
            }

            var result = _mapper.Map<List<TuitionModel>> (tuitionEntity);
            return Result<List<TuitionModel>>.Success (result);
        }
    }
}
