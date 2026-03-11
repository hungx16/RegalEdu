using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetStudentTimetableQuery : IRequest<Result<List<StudentTimetableItemModel>>>
    {
        public required string StudentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetStudentTimetableQueryHandler : IRequestHandler<GetStudentTimetableQuery, Result<List<StudentTimetableItemModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public GetStudentTimetableQueryHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<List<StudentTimetableItemModel>>> Handle(GetStudentTimetableQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result<List<StudentTimetableItemModel>>.Failure(_localizer["InvalidIdFormat"]);
            }

            var fromDate = (request.FromDate ?? DateTime.Today).Date;
            var toDate = (request.ToDate ?? fromDate.AddDays(7)).Date;
            if (toDate < fromDate)
            {
                (fromDate, toDate) = (toDate, fromDate);
            }

            var classIds = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId && e.ClassId != null && !e.IsDeleted)
                .Select(e => e.ClassId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!classIds.Any())
            {
                return Result<List<StudentTimetableItemModel>>.Success(new List<StudentTimetableItemModel>());
            }

            var schedules = await _context.ClassSchedule
                .AsNoTracking()
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Where(cs =>
                    classIds.Contains(cs.ClassId) &&
                    !cs.IsDeleted &&
                    cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled &&
                    cs.Date >= fromDate &&
                    cs.Date <= toDate)
                .OrderBy(cs => cs.Date)
                .ThenBy(cs => cs.StartTime ?? TimeSpan.Zero)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
            {
                return Result<List<StudentTimetableItemModel>>.Success(new List<StudentTimetableItemModel>());
            }

            var scheduleIds = schedules.Select(cs => cs.Id).ToList();
            var attendanceLookup = await _context.ClassAttendent
                .AsNoTracking()
                .Where(ca => !ca.IsDeleted && ca.StudentId == studentId && scheduleIds.Contains(ca.ClassScheduleId))
                .Select(ca => new { ca.ClassScheduleId, ca.StudentParticipationStatus })
                .ToDictionaryAsync(x => x.ClassScheduleId, x => x.StudentParticipationStatus, cancellationToken);

            var result = schedules
                .Select(cs => new StudentTimetableItemModel
                {
                    ClassScheduleId = cs.Id,
                    ClassId = cs.ClassId,
                    ClassName = cs.Class?.ClassName,
                    CourseId = cs.Class?.CourseId ?? Guid.Empty,
                    CourseName = cs.Class?.Course?.CourseName,
                    SessionDate = cs.Date,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    ClassScheduleStatus = cs.ClassScheduleStatus,
                    StudentParticipationStatus = attendanceLookup.TryGetValue(cs.Id, out var participation) ? participation : null
                })
                .ToList();

            return Result<List<StudentTimetableItemModel>>.Success(result);
        }
    }
}
