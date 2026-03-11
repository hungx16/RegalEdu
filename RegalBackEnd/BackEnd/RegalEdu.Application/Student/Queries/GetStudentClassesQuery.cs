using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetStudentClassesQuery : IRequest<Result<List<StudentClassItemModel>>>
    {
        public required string StudentId { get; set; }
    }

    public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, Result<List<StudentClassItemModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public GetStudentClassesQueryHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<List<StudentClassItemModel>>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result<List<StudentClassItemModel>>.Failure(_localizer["InvalidIdFormat"]);
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Teacher)
                        .ThenInclude(t => t.ApplicationUser)
                .Where(e => e.StudentId == studentId && e.ClassId != null && !e.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
            {
                return Result<List<StudentClassItemModel>>.Success(new List<StudentClassItemModel>());
            }

            var classIds = enrollments
                .Where(e => e.ClassId.HasValue)
                .Select(e => e.ClassId!.Value)
                .Distinct()
                .ToList();

            var today = DateTime.Today;

            var sessionTotals = await _context.ClassSchedule
                .AsNoTracking()
                .Where(cs => classIds.Contains(cs.ClassId) && !cs.IsDeleted && cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled)
                .GroupBy(cs => cs.ClassId)
                .Select(g => new { ClassId = g.Key, Total = g.Count() })
                .ToListAsync(cancellationToken);

            var attendanceCounts = await _context.ClassAttendent
                .AsNoTracking()
                .Where(ca => ca.StudentId == studentId && !ca.IsDeleted)
                .Join(
                    _context.ClassSchedule.AsNoTracking()
                        .Where(cs => classIds.Contains(cs.ClassId) && !cs.IsDeleted && cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled),
                    ca => ca.ClassScheduleId,
                    cs => cs.Id,
                    (ca, cs) => new { cs.ClassId, ca.StudentParticipationStatus })
                .GroupBy(x => x.ClassId)
                .Select(g => new
                {
                    ClassId = g.Key,
                    Attended = g.Count(x => x.StudentParticipationStatus == StudentParticipationStatus.Present)
                })
                .ToListAsync(cancellationToken);

            var upcomingSessions = await _context.ClassSchedule
                .AsNoTracking()
                .Where(cs =>
                    classIds.Contains(cs.ClassId) &&
                    !cs.IsDeleted &&
                    cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled &&
                    cs.Date >= today)
                .OrderBy(cs => cs.Date)
                .ThenBy(cs => cs.StartTime ?? TimeSpan.Zero)
                .Select(cs => new
                {
                    cs.Id,
                    cs.ClassId,
                    cs.Date,
                    cs.StartTime,
                    cs.EndTime,
                    cs.ClassScheduleStatus
                })
                .ToListAsync(cancellationToken);

            var nextSessionLookup = upcomingSessions
                .GroupBy(cs => cs.ClassId)
                .ToDictionary(g => g.Key, g => g.First());

            var nextScheduleIds = nextSessionLookup.Values.Select(s => s.Id).ToList();

            var nextParticipationLookup = nextScheduleIds.Count == 0
                ? new Dictionary<Guid, StudentParticipationStatus>()
                : await _context.ClassAttendent
                    .AsNoTracking()
                    .Where(ca => nextScheduleIds.Contains(ca.ClassScheduleId) && ca.StudentId == studentId && !ca.IsDeleted)
                    .Select(ca => new { ca.ClassScheduleId, ca.StudentParticipationStatus })
                    .ToDictionaryAsync(x => x.ClassScheduleId, x => x.StudentParticipationStatus, cancellationToken);

            var totalLookup = sessionTotals.ToDictionary(x => x.ClassId, x => x.Total);
            var attendedLookup = attendanceCounts.ToDictionary(x => x.ClassId, x => x.Attended);

            var result = enrollments
                .Where(e => e.ClassId.HasValue && e.Class != null)
                .GroupBy(e => e.ClassId!.Value)
                .Select(g =>
                {
                    var enrollment = g.First();
                    var classEntity = enrollment.Class!;
                    var classId = enrollment.ClassId!.Value;

                    var total = totalLookup.TryGetValue(classId, out var totalSessions) ? totalSessions : 0;
                    var attended = attendedLookup.TryGetValue(classId, out var attendedSessions) ? attendedSessions : 0;
                    var percent = total > 0 ? Math.Round(attended * 100.0 / total, 2) : 0;

                    nextSessionLookup.TryGetValue(classId, out var nextSession);
                    var participation = nextSession != null && nextParticipationLookup.TryGetValue(nextSession.Id, out var participationStatus)
                        ? participationStatus
                        : (StudentParticipationStatus?)null;

                    return new StudentClassItemModel
                    {
                        ClassId = classId,
                        ClassName = classEntity.ClassName,
                        ClassCode = classEntity.ClassCode,
                        CourseId = classEntity.CourseId,
                        CourseName = classEntity.Course?.CourseName,
                        ClassStatus = classEntity.ClassStatus,
                        Method = classEntity.Method,
                        StartDate = enrollment.StartDate ?? classEntity.StartDate,
                        EndDate = enrollment.EndDate ?? classEntity.EndDate,
                        TeacherId = classEntity.TeacherId,
                        TeacherName = classEntity.Teacher?.ApplicationUser?.FullName,
                        StudentCourseStatus = enrollment.StudentCourseStatus,
                        PaymentCourseStatus = enrollment.PaymentCourseStatus,
                        TotalSessions = total,
                        AttendedSessions = attended,
                        ProgressPercent = percent,
                        NextClassScheduleId = nextSession?.Id,
                        NextSessionDate = nextSession?.Date,
                        NextSessionStartTime = nextSession?.StartTime,
                        NextSessionEndTime = nextSession?.EndTime,
                        NextClassScheduleStatus = nextSession?.ClassScheduleStatus,
                        NextStudentParticipationStatus = participation
                    };
                })
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.ClassName)
                .ToList();

            return Result<List<StudentClassItemModel>>.Success(result);
        }
    }
}
