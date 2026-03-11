using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Queries
{
    public class GetPublishedLearningRoadMapByIdQuery : IRequest<Result<LearningRoadMapModel>>
    {
        public required string Id { get; set; }

    }

    public class GetPublishedLearningRoadMapByIdQueryHandler : IRequestHandler<GetPublishedLearningRoadMapByIdQuery, Result<LearningRoadMapModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _Mapper;
        private readonly ILocalizationService _localizer;

        public GetPublishedLearningRoadMapByIdQueryHandler(IRegalEducationDbContext context, IMapper Mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<LearningRoadMapModel>> Handle(GetPublishedLearningRoadMapByIdQuery request, CancellationToken cancellationToken)
        {
            var lang = _localizer.GetCurrentLanguage ( );

            var learningRoadMap = await _context.LearningRoadMaps
                .Include (c => c.Courses)
                .Include (c => c.Images)
                .Include (t => t.AgeGroup)
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (learningRoadMap == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.LearningRoadMap], request.Id);
                return Result<LearningRoadMapModel>.Failure (msg);
            }

            var result = _Mapper.Map<LearningRoadMapModel> (learningRoadMap);

            // Ngôn ngữ hiện tại là EN
            if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
            {
                // Chỉ lấy bản ghi song ngữ
                result.LearningRoadMapName =
                    !string.IsNullOrWhiteSpace (result.EnLearningRoadMapName) ? result.EnLearningRoadMapName : result.LearningRoadMapName;
                result.Description =
                    !string.IsNullOrWhiteSpace (result.EnDescription) ? result.EnDescription : result.Description;

                // Duyệt qua các course để lấy song ngữ nếu có
                // Alternative: iterate backwards to safely remove
                if (result.Courses != null && result.Courses.Any ( ))
                {
                    for (int i = result.Courses.Count - 1; i >= 0; i--)
                    {
                        var course = result.Courses[i];
                        if (course.IsMultilingual)
                        {
                            if (!string.IsNullOrWhiteSpace (course.EnCourseName)) course.CourseName = course.EnCourseName;
                            if (!string.IsNullOrWhiteSpace (course.EnDescription)) course.Description = course.EnDescription;
                            if (!string.IsNullOrWhiteSpace (course.EnCourseContent)) course.CourseContent = course.EnCourseContent;
                            if (!string.IsNullOrWhiteSpace (course.EnCourseKey)) course.CourseKey = course.EnCourseKey;
                            if (!string.IsNullOrWhiteSpace (course.EnDuration)) course.Duration = course.EnDuration;
                        }
                        else
                        {
                            result.Courses.RemoveAt (i);
                        }
                    }
                }
                if (result.AgeGroup != null)
                {
                    result.AgeGroup.CategoryName = !string.IsNullOrWhiteSpace (result.AgeGroup.EnCategoryName)
                                                 ? result.AgeGroup.EnCategoryName
                                                 : result.AgeGroup.CategoryName;
                }
            }

            return Result<LearningRoadMapModel>.Success (result);
        }

    }
}