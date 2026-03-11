using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Student.Commands
{
    public class SubmitStudentTeacherFeedbackCommand : IRequest<Result>
    {
        public required string StudentId { get; set; }
        public required string ClassScheduleId { get; set; }
        public string? EvaluateContent { get; set; }
        public double? StarRating { get; set; }
    }

    public class SubmitStudentTeacherFeedbackCommandHandler : IRequestHandler<SubmitStudentTeacherFeedbackCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public SubmitStudentTeacherFeedbackCommandHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(SubmitStudentTeacherFeedbackCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result.Failure(_localizer["InvalidIdFormat"]);
            }
            if (!Guid.TryParse(request.ClassScheduleId, out var scheduleId))
            {
                return Result.Failure(_localizer["InvalidIdFormat"]);
            }
            if (request.StarRating.HasValue && (request.StarRating < 1 || request.StarRating > 5))
            {
                return Result.Failure(_localizer.Format("StarRatingRange", 1, 5));
            }

            var schedule = await _context.ClassSchedule
                .Include(cs => cs.Class)
                    .ThenInclude(c => c!.Teacher)
                        .ThenInclude(t => t!.ApplicationUser)
                .FirstOrDefaultAsync(cs => cs.Id == scheduleId && !cs.IsDeleted, cancellationToken);

            if (schedule == null || schedule.Class == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer["ClassSchedule"], request.ClassScheduleId));
            }

            var classId = schedule.ClassId;
            var teacherId = schedule.TeacherId ?? schedule.Class.TeacherId;
            if (teacherId == null || teacherId == Guid.Empty)
            {
                return Result.Failure(_localizer["TeacherRequired"]);
            }

            var enrolled = await _context.Enrollments
                .AnyAsync(e => !e.IsDeleted && e.StudentId == studentId && e.ClassId == classId, cancellationToken);

            if (!enrolled)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, _localizer["Enrollment"]));
            }

            // optional: only allow feedback after session date
            if (schedule.Date > DateTime.Today)
            {
                return Result.Failure(_localizer["ClassScheduleNotStarted"]);
            }

            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
            var evaluateName = student?.FullName;

            var existing = await _context.EvaluateTeachers
                .FirstOrDefaultAsync(et =>
                    !et.IsDeleted &&
                    et.StudentId == studentId &&
                    et.ClassScheduleId == scheduleId, cancellationToken);

            if (existing == null)
            {
                var feedback = new RegalEdu.Domain.Entities.EvaluateTeacher
                {
                    TeacherId = teacherId.Value,
                    StudentId = studentId,
                    ClassId = classId,
                    ClassScheduleId = scheduleId,
                    EvaluateDate = DateTime.UtcNow,
                    StarRating = request.StarRating,
                    ResponseContent = request.EvaluateContent,
                    EvaluateType = "Student",
                    EvaluateName = evaluateName
                };

                await _context.EvaluateTeachers.AddAsync(feedback, cancellationToken);
            }
            else
            {
                existing.StarRating = request.StarRating;
                existing.ResponseContent = request.EvaluateContent;
                existing.EvaluateName = evaluateName;
                existing.EvaluateDate = DateTime.UtcNow;
                existing.EvaluateType = "Student";
                existing.TeacherId = teacherId.Value;
                existing.ClassId = classId;
            }

            var saved = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (saved)
            {
                return Result.Success(_localizer["EvaluateTeacherResponseSuccess"]);
            }

            return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.EvaluateTeacher]));
        }
    }
}
