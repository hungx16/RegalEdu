using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetCoursesWithClassByStudentIdQuery : IRequest<Result<List<StudentCourseProgressModel>>>
    {
        public required string StudentId { get; set; }
    }

    public class GetCoursesWithClassByStudentIdQueryHandler : IRequestHandler<GetCoursesWithClassByStudentIdQuery, Result<List<StudentCourseProgressModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public GetCoursesWithClassByStudentIdQueryHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<List<StudentCourseProgressModel>>> Handle(GetCoursesWithClassByStudentIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result<List<StudentCourseProgressModel>>.Failure(_localizer["InvalidIdFormat"]);
            }

            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Where(e => e.StudentId == studentId && e.ClassId != null && e.CourseId != null && !e.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
            {
                return Result<List<StudentCourseProgressModel>>.Success(new List<StudentCourseProgressModel>());
            }

            var classIds = enrollments
                .Where(e => e.ClassId.HasValue)
                .Select(e => e.ClassId!.Value)
                .Distinct()
                .ToList();

            var upcomingSchedules = await _context.ClassSchedule
                .AsNoTracking()
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Where(cs =>
                    classIds.Contains(cs.ClassId) &&
                    !cs.IsDeleted &&
                    cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled &&
                    (cs.Date > today ||
                     (cs.Date == today && (cs.StartTime ?? TimeSpan.Zero) >= nowTime)))
                .ToListAsync(cancellationToken);

            // Pick the nearest upcoming session per class to avoid duplicate items
            var nextScheduleLookup = upcomingSchedules
                .GroupBy(cs => cs.ClassId)
                .Select(g => g
                    .OrderBy(cs => cs.Date)
                    .ThenBy(cs => cs.StartTime ?? TimeSpan.Zero)
                    .First())
                .ToDictionary(cs => cs.ClassId, cs => cs);

            var sessionTotals = await _context.ClassSchedule
                .AsNoTracking()
                .Where(cs => classIds.Contains(cs.ClassId) && !cs.IsDeleted && cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled)
                .GroupBy(cs => cs.ClassId)
                .Select(g => new { ClassId = g.Key, Total = g.Count() })
                .ToListAsync(cancellationToken);

            var sessionAttended = await _context.ClassAttendent
                .AsNoTracking()
                .Where(ca => ca.StudentId == studentId && !ca.IsDeleted && ca.StudentParticipationStatus == StudentParticipationStatus.Present)
                .Join(
                    _context.ClassSchedule.AsNoTracking()
                        .Where(cs => classIds.Contains(cs.ClassId) && !cs.IsDeleted && cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled),
                    attendent => attendent.ClassScheduleId,
                    schedule => schedule.Id,
                    (attendent, schedule) => schedule.ClassId)
                .GroupBy(classId => classId)
                .Select(g => new { ClassId = g.Key, Attended = g.Count() })
                .ToListAsync(cancellationToken);

            var totalLookup = sessionTotals.ToDictionary(x => x.ClassId, x => x.Total);
            var attendedLookup = sessionAttended.ToDictionary(x => x.ClassId, x => x.Attended);

            var result = enrollments
                .Where(e => e.ClassId.HasValue)
                .GroupBy(e => e.ClassId!.Value)
                .Select(g =>
                {
                    var enrollment = g.First();
                    var classId = enrollment.ClassId!.Value;
                    var classEntity = enrollment.Class;
                    var courseEntity = enrollment.Course;
                    var total = totalLookup.TryGetValue(classId, out var t) ? t : 0;
                    var attended = attendedLookup.TryGetValue(classId, out var a) ? a : 0;
                    var percent = total > 0 ? Math.Round(attended * 100.0 / total, 2) : 0;
                    nextScheduleLookup.TryGetValue(classId, out var nextSession);

                    if (nextSession == null)
                    {
                        return null;
                    }

                    return new StudentCourseProgressModel
                    {
                        CourseId = courseEntity?.Id ?? Guid.Empty,
                        CourseName = courseEntity?.CourseName,
                        ClassId = classId,
                        ClassName = classEntity?.ClassName,
                        TotalSessions = total,
                        AttendedSessions = attended,
                        ProgressPercent = percent,
                        ClassScheduleId = nextSession.Id,
                        SessionDate = nextSession.Date,
                        StartTime = nextSession.StartTime,
                        EndTime = nextSession.EndTime,
                        ClassScheduleStatus = nextSession.ClassScheduleStatus
                    };
                })
                .Where(x => x != null)
                .OrderBy(x => x!.SessionDate)
                .ThenBy(x => x!.StartTime ?? TimeSpan.Zero)
                .Select(x => x!)
                .ToList();

            return Result<List<StudentCourseProgressModel>>.Success(result);
        }
    }
}
