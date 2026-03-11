using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Course.Queries
{
    public class GetAllCoursesQuery : IRequest<Result<List<CourseModel>>> { }

    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Result<List<CourseModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCoursesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        public async Task<Result<List<CourseModel>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _context.Courses
                .Include(c => c.LearningRoadMap)
                .Include(c => c.DetailRegisterStudies)
                .Include(c => c.Tuitions)
                    .ThenInclude(t => t.CourseLessons)
                        .ThenInclude(l => l.HomeworkAttachments)
                .Include(c => c.Tuitions)
                    .ThenInclude(t => t.CourseLessons)
                        .ThenInclude(l => l.ReferenceAttachments)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = _mapper.Map<List<CourseModel>>(courses);

            return Result<List<CourseModel>>.Success(result);
        }
    }
}
