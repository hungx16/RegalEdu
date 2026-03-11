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
    public class AddCourseCommand : IRequest<Result>
    {
        public required CourseModel CourseModel { get; set; }
    }

    public class AddCourseCommandHandler : IRequestHandler<AddCourseCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        private const string HomeworkFolder = "course-lessons/homework";
        private const string ReferenceFolder = "course-lessons/reference";

        public AddCourseCommandHandler(
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

        public async Task<Result> Handle(AddCourseCommand request, CancellationToken cancellationToken)
        {
            if (_context is not DbContext dbContext)
                throw new InvalidOperationException(_localizer[LocalizationKey.InvalidDbContextInstance]);

            // ✅ 1. Map Course
            var course = _mapper.Map<Domain.Entities.Course>(request.CourseModel);
            // Ensure we don't keep AutoMapper-mapped tuitions (they will be created below with generated codes)
            course.Tuitions = new List<Domain.Entities.Tuition>();

            // ✅ 2. Tính lại OrdinalNumber
            var roadmap = await _context.LearningRoadMaps
                .FirstOrDefaultAsync(lr => lr.Id == course.LearningRoadMapId, cancellationToken);
            var roadmapSeq = roadmap?.Order ?? 0;
            course.OrdinalNumber = roadmapSeq + (course.Sequence / 10.0f);

            // ✅ 3. Xử lý danh sách Tuition (và Lessons)
            if (request.CourseModel.Tuitions != null && request.CourseModel.Tuitions.Any())
            {
                foreach (var dto in request.CourseModel.Tuitions)
                {
                    var newTuition = new RegalEdu.Domain.Entities.Tuition
                    {
                        Id = Guid.NewGuid(),
                        CourseId = course.Id,
                        TuitionName = dto.TuitionName,
                        ClassTypeId = dto.ClassTypeId,
                        DurationHours = dto.DurationHours,
                        MinHours = dto.MinHours,
                        TotalMonths = dto.TotalMonths,
                        Unit = dto.Unit,
                        TuitionFee = dto.TuitionFee,
                        TuitionCode = await AutoCodeHelper.GenerateCodeAsync(
                            new AutoCodeInfo
                            {
                                TableName = "Tuition",
                                ColumnName = "TuitionCode",
                                Prefix = "TU",
                                Length = 4
                            },
                            dbContext
                        ),
                        CourseLessons = new List<RegalEdu.Domain.Entities.CourseLesson>()
                    };

                    // ✅ Thêm Lessons (không reuse ID từ FE)
                    if (dto.CourseLessons != null && dto.CourseLessons.Any())
                    {
                        foreach (var lessonDto in dto.CourseLessons)
                        {
                            var lessonId = Guid.NewGuid();
                            var newLesson = new RegalEdu.Domain.Entities.CourseLesson
                            {
                                Id = lessonId, // ⚠️ luôn tạo mới ID
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
                                    lessonId,
                                    HomeworkFolder,
                                    isHomework: true,
                                    cancellationToken);

                                newLesson.ReferenceAttachments = await BuildAttachmentsAsync(
                                    lessonDto.ReferenceAttachments,
                                    lessonId,
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

                    course.Tuitions.Add(newTuition);
                }
            }

            // ✅ 4. Thêm vào context (chỉ add Course, EF sẽ cascade xuống Tuition & Lesson)
            await _context.Courses.AddAsync(course, cancellationToken);

            // ✅ 5. Lưu
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                return Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Course));
            }
            else
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Course));
            }
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

    }
}



