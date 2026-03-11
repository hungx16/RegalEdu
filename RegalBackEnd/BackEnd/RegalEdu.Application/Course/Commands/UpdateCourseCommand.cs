using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;
using System.IO;

namespace RegalEdu.Application.Course.Commands
{
    public class UpdateCourseCommand : IRequest<Result>
    {
        public required CourseModel CourseModel { get; set; }
    }

    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        private const string HomeworkFolder = "course-lessons/homework";
        private const string ReferenceFolder = "course-lessons/reference";

        public UpdateCourseCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer,
            IFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var model = request.CourseModel;

            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

            if (existingCourse == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Course));

            // 🔹 1️⃣ Xóa Tuition & Lesson bị xóa
            if (model.DeletedTuitionIds != null && model.DeletedTuitionIds.Any())
            {
                var tuitionIds = model.DeletedTuitionIds.ToList();

                // Xóa tất cả CourseLesson trước
                var relatedLessons = await _context.CourseLessons
                    .Include(l => l.HomeworkAttachments)
                    .Include(l => l.ReferenceAttachments)
                    .Where(l => tuitionIds.Contains(l.TuitionId))
                    .ToListAsync(cancellationToken);

                if (relatedLessons.Any())
                {
                    foreach (var lesson in relatedLessons)
                    {
                        await RemoveAttachmentsAsync(lesson.HomeworkAttachments, cancellationToken);
                        await RemoveAttachmentsAsync(lesson.ReferenceAttachments, cancellationToken);
                    }

                    _context.CourseLessons.RemoveRange(relatedLessons);
                }

                // Sau đó xóa Tuition
                var relatedTuitions = await _context.Tuition
                    .Where(t => tuitionIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                if (relatedTuitions.Any())
                    _context.Tuition.RemoveRange(relatedTuitions);

                await _context.SaveChangesAsync(cancellationToken);
            }

            // 🔹 2️⃣ Cập nhật hoặc thêm mới Tuition & Lesson
            if (model.Tuitions != null && model.Tuitions.Any())
            {
                foreach (var dto in model.Tuitions)
                {
                    var existingTuition = await _context.Tuition
                        .Include(t => t.CourseLessons)
                            .ThenInclude(l => l.HomeworkAttachments)
                        .Include(t => t.CourseLessons)
                            .ThenInclude(l => l.ReferenceAttachments)
                        .FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

                    if (existingTuition != null)
                    {
                        // ✏️ Update Tuition
                        existingTuition.TuitionName = dto.TuitionName;
                        existingTuition.ClassTypeId = dto.ClassTypeId;
                        existingTuition.DurationHours = dto.DurationHours;
                        existingTuition.MinHours = dto.MinHours;
                        existingTuition.TotalMonths = dto.TotalMonths;
                        existingTuition.Unit = dto.Unit;
                        existingTuition.TuitionFee = dto.TuitionFee;
                        existingTuition.UpdatedAt = DateTime.Now;

                        // ✏️ Update Lessons
                        if (dto.CourseLessons != null)
                        {
                            var requestLessonIds = dto.CourseLessons
                                .Where(l => l.Id != Guid.Empty)
                                .Select(l => l.Id).ToHashSet();

                            // ❌ Xóa Lesson không còn trong request
                            var toRemoveLessons = existingTuition.CourseLessons
                                .Where(l => !requestLessonIds.Contains(l.Id))
                                .ToList();

                            if (toRemoveLessons.Any())
                            {
                                foreach (var lesson in toRemoveLessons)
                                {
                                    await RemoveAttachmentsAsync(lesson.HomeworkAttachments, cancellationToken);
                                    await RemoveAttachmentsAsync(lesson.ReferenceAttachments, cancellationToken);
                                }

                                _context.CourseLessons.RemoveRange(toRemoveLessons);
                            }

                            // 🔁 Cập nhật / thêm mới Lesson
                            foreach (var lessonDto in dto.CourseLessons)
                            {
                                var existingLesson = existingTuition.CourseLessons
                                    .FirstOrDefault(l => l.Id == lessonDto.Id);

                                if (existingLesson != null)
                                {
                                    existingLesson.SessionName = lessonDto.SessionName;
                                    existingLesson.LectureTypeId = lessonDto.LectureTypeId;
                                    existingLesson.LessonName = lessonDto.LessonName;
                                    existingLesson.Objective = lessonDto.Objective;
                                    existingLesson.Content = lessonDto.Content;
                                    existingLesson.Homework = lessonDto.Homework;
                                    existingLesson.Reference = lessonDto.Reference;
                                    try
                                    {
                                        await SyncAttachmentsAsync(
                                            existingLesson,
                                            lessonDto.HomeworkAttachments,
                                            HomeworkFolder,
                                            isHomework: true,
                                            cancellationToken);

                                        await SyncAttachmentsAsync(
                                            existingLesson,
                                            lessonDto.ReferenceAttachments,
                                            ReferenceFolder,
                                            isHomework: false,
                                            cancellationToken);
                                    }
                                    catch (Exception ex)
                                    {
                                        var msg = _localizer.Format(LocalizationKey.ERR_FILE_UPLOAD_FAILED, string.Empty, Functions.GetFullExceptionMessage(ex));
                                        return Result.Failure(msg);
                                    }
                                }
                                else
                                {
                                    var newLessonId = Guid.NewGuid();
                                    var newLesson = new RegalEdu.Domain.Entities.CourseLesson
                                    {
                                        Id = newLessonId,
                                        TuitionId = existingTuition.Id,
                                        SessionName = lessonDto.SessionName,
                                        LectureTypeId = lessonDto.LectureTypeId,
                                        LessonName = lessonDto.LessonName,
                                        Objective = lessonDto.Objective,
                                        Content = lessonDto.Content,
                                        Homework = lessonDto.Homework,
                                        Reference = lessonDto.Reference
                                    };
                                    try
                                    {
                                        newLesson.HomeworkAttachments = await BuildAttachmentsAsync(
                                            lessonDto.HomeworkAttachments,
                                            newLessonId,
                                            HomeworkFolder,
                                            isHomework: true,
                                            cancellationToken);

                                        newLesson.ReferenceAttachments = await BuildAttachmentsAsync(
                                            lessonDto.ReferenceAttachments,
                                            newLessonId,
                                            ReferenceFolder,
                                            isHomework: false,
                                            cancellationToken);
                                    }
                                    catch (Exception ex)
                                    {
                                        var msg = _localizer.Format(LocalizationKey.ERR_FILE_UPLOAD_FAILED, string.Empty, Functions.GetFullExceptionMessage(ex));
                                        return Result.Failure(msg);
                                    }
                                    await _context.CourseLessons.AddAsync(newLesson, cancellationToken);
                                }
                            }
                        }
                    }
                    else
                    {
                        // 🆕 Thêm mới Tuition + Lesson
                        var newTuition = _mapper.Map<RegalEdu.Domain.Entities.Tuition>(dto);
                        newTuition.CourseId = model.Id;
                        newTuition.TuitionCode = await AutoCodeHelper.GenerateCodeAsync(
                            new AutoCodeInfo
                            {
                                TableName = "Tuition",
                                ColumnName = "TuitionCode",
                                Prefix = "TU",
                                Length = 4
                            },
                            (DbContext)_context
                        );

                        // Map Lesson nếu có
                        if (dto.CourseLessons != null && dto.CourseLessons.Any())
                        {
                            newTuition.CourseLessons = new List<RegalEdu.Domain.Entities.CourseLesson>();
                            foreach (var lessonDto in dto.CourseLessons)
                            {
                                var newLessonId = Guid.NewGuid();
                                var newLesson = new RegalEdu.Domain.Entities.CourseLesson
                                {
                                    Id = newLessonId,
                                    TuitionId = newTuition.Id,
                                    SessionName = lessonDto.SessionName,
                                    LectureTypeId = lessonDto.LectureTypeId,
                                    LessonName = lessonDto.LessonName,
                                    Objective = lessonDto.Objective,
                                    Content = lessonDto.Content,
                                    Homework = lessonDto.Homework,
                                    Reference = lessonDto.Reference
                                };

                                try
                                {
                                    newLesson.HomeworkAttachments = await BuildAttachmentsAsync(
                                        lessonDto.HomeworkAttachments,
                                        newLessonId,
                                        HomeworkFolder,
                                        isHomework: true,
                                        cancellationToken);

                                    newLesson.ReferenceAttachments = await BuildAttachmentsAsync(
                                        lessonDto.ReferenceAttachments,
                                        newLessonId,
                                        ReferenceFolder,
                                        isHomework: false,
                                        cancellationToken);
                                }
                                catch (Exception ex)
                                {
                                    var msg = _localizer.Format(LocalizationKey.ERR_FILE_UPLOAD_FAILED, string.Empty, Functions.GetFullExceptionMessage(ex));
                                    return Result.Failure(msg);
                                }

                                newTuition.CourseLessons.Add(newLesson);
                            }
                        }

                        await _context.Tuition.AddAsync(newTuition, cancellationToken);
                    }
                }
            }


            existingCourse.CourseCode = model.CourseCode?.Trim() ?? existingCourse.CourseCode;
            existingCourse.CourseName = model.CourseName?.Trim() ?? existingCourse.CourseName;
            existingCourse.EnCourseName = model.EnCourseName?.Trim();
            existingCourse.Description = model.Description?.Trim();
            existingCourse.EnDescription = model.EnDescription?.Trim();
            existingCourse.CourseContent = model.CourseContent;
            existingCourse.EnCourseContent = model.EnCourseContent;
            existingCourse.CourseKey = model.CourseKey;
            existingCourse.EnCourseKey = model.EnCourseKey;
            existingCourse.Reference = model.Reference?.Trim();

            existingCourse.IsMultilingual = model.IsMultilingual;
            existingCourse.IsPublish = model.IsPublish;

            existingCourse.Sequence = model.Sequence;
            existingCourse.MinAvgScore = model.MinAvgScore;

            existingCourse.LearningRoadMapId = model.LearningRoadMapId;

            existingCourse.MidExamIds = model.MidExamIds;
            existingCourse.FinalExamIds = model.FinalExamIds;

            existingCourse.CommitmentOutputType = model.CommitmentOutputType;
            existingCourse.CommitmentLevel = model.CommitmentLevel;

            existingCourse.Duration = model.Duration;
            existingCourse.EnDuration = model.EnDuration;

            existingCourse.NumberOfStudents = model.NumberOfStudents;
            existingCourse.VotingRate = model.VotingRate;
            existingCourse.Status = model.Status;


            // Tính lại OrdinalNumber
            var roadmap = await _context.LearningRoadMaps
                .FirstOrDefaultAsync(lr => lr.Id == existingCourse.LearningRoadMapId, cancellationToken);
            var roadmapSeq = roadmap?.Order ?? 0;
            existingCourse.OrdinalNumber = roadmapSeq + (existingCourse.Sequence / 10.0f);

            _context.Courses.Update(existingCourse);

            // 🔹 4️⃣ Lưu thay đổi
            try
            {
                var affected = await _context.SaveChangesAsync(cancellationToken);
                return affected > 0
                    ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Course))
                    : Result.Success(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Course));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_CONCURRENCY));
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        private async Task SyncAttachmentsAsync(
            Domain.Entities.CourseLesson courseLesson,
            IEnumerable<AttachmentModel>? models,
            string destFolder,
            bool isHomework,
            CancellationToken cancellationToken)
        {
            if (models == null)
                return;

            var existing = isHomework
                ? (courseLesson.HomeworkAttachments ?? new List<Attachment>())
                : (courseLesson.ReferenceAttachments ?? new List<Attachment>());

            var incomingList = models.ToList();
            var incomingIds = incomingList
                .Where(x => x.Id != Guid.Empty)
                .Select(x => x.Id)
                .ToHashSet();

            var incomingPaths = incomingList
                .Where(x => !string.IsNullOrWhiteSpace(x.Path))
                .Select(x => x.Path!.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var toRemove = existing
                .Where(a => !incomingIds.Contains(a.Id) && !incomingPaths.Contains(a.Path))
                .ToList();

            if (toRemove.Any())
            {
                foreach (var remove in toRemove)
                {
                    if (!string.IsNullOrWhiteSpace(remove.Path))
                    {
                        try { await _fileService.DeleteFileAsync(remove.Path); } catch { /* ignore */ }
                    }
                }

                _context.Attachments.RemoveRange(toRemove);
                foreach (var remove in toRemove)
                {
                    existing.Remove(remove);
                }
            }

            foreach (var model in incomingList)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var path = model.Path?.Trim();
                if (string.IsNullOrWhiteSpace(path) && model.Id == Guid.Empty)
                    continue;

                if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                {
                    path = await _fileService.MoveFileAsync(path, destFolder);
                }

                var existingAttachment = model.Id != Guid.Empty
                    ? existing.FirstOrDefault(a => a.Id == model.Id)
                    : existing.FirstOrDefault(a => !string.IsNullOrWhiteSpace(path) && string.Equals(a.Path, path, StringComparison.OrdinalIgnoreCase));

                if (existingAttachment == null)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    var fileName = !string.IsNullOrWhiteSpace(model.FileName)
                        ? model.FileName
                        : Path.GetFileName(path);

                    var attachment = new Attachment
                    {
                        Path = path,
                        FileName = fileName
                    };

                    if (isHomework)
                        attachment.CourseLessonHomeworkId = courseLesson.Id;
                    else
                        attachment.CourseLessonReferenceId = courseLesson.Id;

                    existing.Add(attachment);
                    _context.Attachments.Add(attachment);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(path) && !string.Equals(existingAttachment.Path, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(existingAttachment.Path))
                    {
                        try { await _fileService.DeleteFileAsync(existingAttachment.Path); } catch { /* ignore */ }
                    }
                    existingAttachment.Path = path;
                }

                if (!string.IsNullOrWhiteSpace(model.FileName))
                {
                    existingAttachment.FileName = model.FileName;
                }
                else if (!string.IsNullOrWhiteSpace(path))
                {
                    existingAttachment.FileName = Path.GetFileName(path);
                }

                if (isHomework)
                    existingAttachment.CourseLessonHomeworkId = courseLesson.Id;
                else
                    existingAttachment.CourseLessonReferenceId = courseLesson.Id;
            }

            if (isHomework)
                courseLesson.HomeworkAttachments = existing;
            else
                courseLesson.ReferenceAttachments = existing;
        }

        private async Task<List<Attachment>> BuildAttachmentsAsync(
            IEnumerable<AttachmentModel>? models,
            Guid courseLessonId,
            string destFolder,
            bool isHomework,
            CancellationToken cancellationToken)
        {
            var list = new List<Attachment>();
            if (models == null)
                return list;

            foreach (var model in models)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var path = model.Path?.Trim();
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (path.StartsWith("temp/", StringComparison.OrdinalIgnoreCase))
                {
                    path = await _fileService.MoveFileAsync(path, destFolder);
                }

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var fileName = !string.IsNullOrWhiteSpace(model.FileName)
                    ? model.FileName
                    : Path.GetFileName(path);

                var attachment = new Attachment
                {
                    Path = path,
                    FileName = fileName
                };

                if (isHomework)
                    attachment.CourseLessonHomeworkId = courseLessonId;
                else
                    attachment.CourseLessonReferenceId = courseLessonId;

                list.Add(attachment);
            }

            return list;
        }

        private async Task RemoveAttachmentsAsync(
            IEnumerable<Attachment>? attachments,
            CancellationToken cancellationToken)
        {
            if (attachments == null)
                return;

            var list = attachments.ToList();
            if (!list.Any())
                return;

            foreach (var attachment in list)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrWhiteSpace(attachment.Path))
                {
                    try { await _fileService.DeleteFileAsync(attachment.Path); } catch { /* ignore */ }
                }
            }

            _context.Attachments.RemoveRange(list);
        }
    }
}
