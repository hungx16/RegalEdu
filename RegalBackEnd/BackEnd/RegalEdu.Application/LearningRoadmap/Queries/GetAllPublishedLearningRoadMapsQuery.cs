using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadmap.Queries;

public class GetAllPublishedLearningRoadMapsQuery : IRequest<Result<List<LearningRoadMapModel>>> { }

public class Handler_GetPublishAll : IRequestHandler<GetAllPublishedLearningRoadMapsQuery, Result<List<LearningRoadMapModel>>>
{
    private readonly IRegalEducationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizer;

    public Handler_GetPublishAll(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
    {
        _context = context ?? throw new ArgumentNullException (nameof (context));
        _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
    }

    public async Task<Result<List<LearningRoadMapModel>>> Handle(GetAllPublishedLearningRoadMapsQuery request, CancellationToken cancellationToken)
    {
        var lang = _localizer.GetCurrentLanguage ( );

        var list = await _context.LearningRoadMaps
            .Include (t => t.AgeGroup)
            .Include (t => t.Courses)
            .Include (t => t.Images)
            .Where (t => t.IsPublish == true && t.Status == Domain.Enums.StatusType.Active)
            .OrderBy (x => x.Order)
            .AsNoTracking ( )
            .ToListAsync (cancellationToken);

        var mapped = _mapper.Map<List<LearningRoadMapModel>> (list);

        if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
        {
            // chỉ lấy những bản ghi hỗ trợ song ngữ
            mapped = mapped
                .Where (item => item.IsMultilingual)
                .Select (item =>
                {
                    // Lộ trình
                    item.LearningRoadMapName = !string.IsNullOrWhiteSpace (item.EnLearningRoadMapName)
                        ? item.EnLearningRoadMapName
                        : item.LearningRoadMapName;

                    item.Description = !string.IsNullOrWhiteSpace (item.EnDescription)
                        ? item.EnDescription
                        : item.Description;

                    // Courses trong lộ trình
                    if (item.Courses != null && item.Courses.Any ( ))
                    {
                        for (int i = item.Courses.Count - 1; i >= 0; i--)
                        {
                            var course = item.Courses[i];
                            if (course.IsMultilingual)
                            {
                                if (!string.IsNullOrWhiteSpace (course.EnCourseName))
                                    course.CourseName = course.EnCourseName;

                                if (!string.IsNullOrWhiteSpace (course.EnDescription))
                                    course.Description = course.EnDescription;

                                if (!string.IsNullOrWhiteSpace (course.EnCourseContent))
                                    course.CourseContent = course.EnCourseContent;

                                if (!string.IsNullOrWhiteSpace (course.EnCourseKey))
                                    course.CourseKey = course.EnCourseKey;

                                if (!string.IsNullOrWhiteSpace (course.EnDuration))
                                    course.Duration = course.EnDuration;
                            }
                            else
                            {
                                item.Courses.RemoveAt (i);
                            }
                        }
                    }
                    if (item.AgeGroup != null)
                    {
                        item.AgeGroup.CategoryName = !string.IsNullOrWhiteSpace (item.AgeGroup.EnCategoryName)
                                                     ? item.AgeGroup.EnCategoryName
                                                     : item.AgeGroup.CategoryName;
                    }

                    return item;
                })
                .ToList ( );
        }

        return Result<List<LearningRoadMapModel>>.Success (mapped);
    }

}