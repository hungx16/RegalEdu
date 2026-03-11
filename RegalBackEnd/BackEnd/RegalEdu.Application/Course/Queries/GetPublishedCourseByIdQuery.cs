using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Course.Queries
{
    public class GetPublishedCourseByIdQuery : IRequest<Result<CourseModel>>
    {
        public required string Id { get; set; }

    }

    public class GetPublishedCourseByIdQueryHandler : IRequestHandler<GetPublishedCourseByIdQuery, Result<CourseModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetPublishedCourseByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<CourseModel>> Handle(GetPublishedCourseByIdQuery request, CancellationToken cancellationToken)
        {
            // Chỉ lấy bản ghi chưa bị xoá mềm (IsDeleted == false)
            //.Include(c => c.DetailRegisterStudies)
            var course = await _context.Courses
                .Include(c => c.LearningRoadMap)

                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted && x.IsPublish==true, cancellationToken);

            if (course == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.Course], request.Id);
                return Result<CourseModel>.Failure (msg);
            }

            var result = _mapper.Map<CourseModel> (course);
            return Result<CourseModel>.Success (result);
        }
    }
}