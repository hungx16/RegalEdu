using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Course.Queries
{
    public class GetAllPublishCoursesQuery : IRequest<Result<List<CourseModel>>>
    {
        public string? LearningRoadMapId { get; set; }
    }

    public class Handler_GetPublishAll : IRequestHandler<GetAllPublishCoursesQuery, Result<List<CourseModel>>>
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

        public async Task<Result<List<CourseModel>>> Handle(GetAllPublishCoursesQuery request, CancellationToken cancellationToken)
        {
            var lang = _localizer.GetCurrentLanguage ( );
            var list = await _context.Courses.Where (t => t.IsPublish == true && t.Status == Domain.Enums.StatusType.Active)
                .OrderBy (x => x.CreatedAt)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);
            if (!string.IsNullOrWhiteSpace (request.LearningRoadMapId))
                list = list.Where (x => x.LearningRoadMapId.ToString ( ) == request.LearningRoadMapId).ToList ( );
            var mapped = _mapper.Map<List<CourseModel>> (list);
            if (lang.Equals ("en", StringComparison.OrdinalIgnoreCase))
            {
                // chỉ lấy những bản ghi hỗ trợ song ngữ
                mapped = mapped
                    .Where (item => item.IsMultilingual)
                    .Select (item =>
                    {
                        return item;
                    })
                    .ToList ( );
            }
            return Result<List<CourseModel>>.Success (mapped);
        }
    }
}