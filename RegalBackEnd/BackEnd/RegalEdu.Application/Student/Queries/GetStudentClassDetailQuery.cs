using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetStudentClassDetailQuery : IRequest<Result<StudentClassDetailModel>>
    {
        public required string StudentId { get; set; }
        public required string ClassId { get; set; }
    }

    public class GetStudentClassDetailQueryHandler : IRequestHandler<GetStudentClassDetailQuery, Result<StudentClassDetailModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public GetStudentClassDetailQueryHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<StudentClassDetailModel>> Handle(GetStudentClassDetailQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result<StudentClassDetailModel>.Failure(_localizer["InvalidIdFormat"]);
            }

            if (!Guid.TryParse(request.ClassId, out var classId))
            {
                return Result<StudentClassDetailModel>.Failure(_localizer["InvalidIdFormat"]);
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Class)!
                    .ThenInclude(c => c!.Course)
                .Include(e => e.Class)!
                    .ThenInclude(c => c!.Teacher)!
                        .ThenInclude(t => t!.ApplicationUser)
                .Where(e => !e.IsDeleted && e.StudentId == studentId && e.ClassId == classId)
                .FirstOrDefaultAsync(cancellationToken);

            if (enrollment?.Class == null)
            {
                return Result<StudentClassDetailModel>.Failure(_localizer["EntityNotFound"]);
            }

            var classEntity = enrollment.Class;

            var schedules = await _context.ClassSchedule
                .AsNoTracking()
                .Include(cs => cs.CourseLesson)
                .Include(cs => cs.Attachment)
                .Include(cs => cs.ClassAttendants.Where(ca => !ca.IsDeleted && ca.StudentId == studentId))
                .Where(cs => cs.ClassId == classId && !cs.IsDeleted)
                .OrderBy(cs => cs.Date)
                .ThenBy(cs => cs.StartTime ?? TimeSpan.Zero)
                .ToListAsync(cancellationToken);

            var totalSessions = schedules.Count(cs => cs.ClassScheduleStatus != ClassScheduleStatus.Cancelled);
            var attendedSessions = schedules
                .Select(cs => cs.ClassAttendants?.FirstOrDefault())
                .Count(att => att != null && att.StudentParticipationStatus == StudentParticipationStatus.Present);
            var progressPercent = totalSessions > 0 ? Math.Round(attendedSessions * 100.0 / totalSessions, 2) : 0;

            var sessions = schedules.Select(cs =>
            {
                var attendent = cs.ClassAttendants?.FirstOrDefault();
                var durationMinutes = cs.StartTime.HasValue && cs.EndTime.HasValue
                    ? (int?)(cs.EndTime.Value - cs.StartTime.Value).TotalMinutes
                    : null;

                var attachments = cs.Attachment != null
                    ? new List<AttachmentModel>
                    {
                        new AttachmentModel
                        {
                            Id = cs.Attachment.Id,
                            FileName = cs.Attachment.FileName,
                            Path = cs.Attachment.Path,
                            CreatedAt = cs.Attachment.CreatedAt,
                            ClassScheduleId = cs.Id
                        }
                    }
                    : new List<AttachmentModel>();

                return new StudentClassSessionModel
                {
                    ClassScheduleId = cs.Id,
                    Date = cs.Date,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    DurationMinutes = durationMinutes,
                    LessonName = cs.CourseLesson?.LessonName,
                    SessionName = cs.CourseLesson?.SessionName,
                    Objective = cs.CourseLesson?.Objective,
                    ClassScheduleStatus = cs.ClassScheduleStatus,
                    StudentParticipationStatus = attendent?.StudentParticipationStatus,
                    StudentHomeworkStatus = attendent?.StudentHomeworkStatus,
                    HomeworkScore = attendent?.HomeworkScore,
                    HomeworkTitle = cs.HomeworkPlusName ?? cs.CourseLesson?.Homework,
                    HomeworkDescription = cs.HomeworkPlusContent ?? cs.CourseLesson?.Homework,
                    Attachments = attachments
                };
            }).ToList();

            var homeworks = sessions
                .Where(s => !string.IsNullOrWhiteSpace(s.HomeworkTitle) || !string.IsNullOrWhiteSpace(s.HomeworkDescription))
                .Select(s => new StudentHomeworkItemModel
                {
                    ClassScheduleId = s.ClassScheduleId,
                    Date = s.Date,
                    Title = s.HomeworkTitle ?? s.SessionName ?? s.LessonName,
                    Description = s.HomeworkDescription,
                    Status = s.StudentHomeworkStatus,
                    Score = s.HomeworkScore,
                    ClassScheduleStatus = s.ClassScheduleStatus
                })
                .ToList();

            var materials = schedules
                .Where(cs => cs.Attachment != null)
                .Select(cs => cs.Attachment!)
                .Select(att => new StudentClassMaterialModel
                {
                    AttachmentId = att.Id,
                    ClassScheduleId = att.ClassScheduleId ?? Guid.Empty,
                    FileName = att.FileName,
                    Path = att.Path,
                    CreatedAt = att.CreatedAt
                })
                .ToList();

            var comments = schedules
                .Select(cs => new { cs, attendent = cs.ClassAttendants?.FirstOrDefault() })
                .Where(x => x.attendent != null && !string.IsNullOrWhiteSpace(x.attendent.Comment))
                .Select(x => new StudentClassCommentModel
                {
                    ClassScheduleId = x.cs.Id,
                    Date = x.cs.Date,
                    Comment = x.attendent!.Comment,
                    Star = x.attendent!.Star,
                    TeacherId = classEntity.TeacherId,
                    ParticipationLevel = x.attendent!.ParticipationLevel,
                    LearningAbsorptionLevel = x.attendent!.LearningAbsorptionLevel,
                    DisciplineLevel = x.attendent!.DisciplineLevel,
                    TeacherName = classEntity.Teacher?.ApplicationUser?.FullName ?? classEntity.Teacher?.TeacherNickname
                })
                .ToList();

            var detail = new StudentClassDetailModel
            {
                ClassId = classEntity.Id,
                ClassName = classEntity.ClassName,
                ClassCode = classEntity.ClassCode,
                CourseId = classEntity.CourseId,
                CourseName = classEntity.Course?.CourseName,
                TeacherId = classEntity.TeacherId,
                TeacherName = classEntity.Teacher?.ApplicationUser?.FullName ?? classEntity.Teacher?.TeacherNickname,
                Method = classEntity.Method,
                ScheduleText = classEntity.ClassSchedule,
                StartDate = enrollment.StartDate ?? classEntity.StartDate,
                EndDate = enrollment.EndDate ?? classEntity.EndDate,
                ClassStatus = classEntity.ClassStatus,
                TotalSessions = totalSessions,
                AttendedSessions = attendedSessions,
                ProgressPercent = progressPercent,
                Sessions = sessions,
                Homeworks = homeworks,
                Materials = materials,
                Comments = comments
            };

            var studentEvaluations = await _context.EvaluateTeachers
                .AsNoTracking()
                .Where(et => !et.IsDeleted && et.StudentId == studentId && et.ClassId == classId)
                .OrderByDescending(et => et.EvaluateDate)
                .Select(et => new StudentTeacherEvaluationModel
                {
                    Id = et.Id,
                    TeacherId = et.TeacherId,
                    ClassId = et.ClassId ?? classId,
                    ClassScheduleId = et.ClassScheduleId ?? Guid.Empty,
                    StudentId = et.StudentId,
                    StudentName = et.EvaluateName,
                    StarRating = et.StarRating,
                    ResponseContent = et.ResponseContent,
                    EvaluateDate = et.EvaluateDate
                })
                .ToListAsync(cancellationToken);

            detail.StudentTeacherEvaluations = studentEvaluations;

            return Result<StudentClassDetailModel>.Success(detail);
        }
    }
}
