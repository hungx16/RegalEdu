using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Course.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Course.Validators
{
    public class AddCourseCommandValidator : AbstractValidator<AddCourseCommand>
    {
        public AddCourseCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1️ Kiểm tra rule cơ bản từ CourseModel
            RuleFor (x => x.CourseModel)
                .SetValidator (new BaseCourseModelValidator (localizer));

            // 2️ Kiểm tra LearningRoadmapId tồn tại trong DB
            RuleFor (x => x.CourseModel.LearningRoadMapId)
                .NotEmpty ( ).WithMessage (localizer["LearningRoadmapIdNotEmpty"])
                .MustAsync (async (id, cancellation) =>
                    id.HasValue &&
                    await dbContext.LearningRoadMaps
                        .AnyAsync (lr => lr.Id == id.Value && !lr.IsDeleted, cancellation)
                )
                .WithMessage (localizer["LearningRoadmapIdNotFound"]);

            // 3️ Kiểm tra Code duy nhất
            RuleFor (x => x.CourseModel.CourseCode)
                .MustAsync (async (code, cancellation) =>
                    !await dbContext.Courses.AnyAsync (c => c.CourseCode == code && !c.IsDeleted, cancellation))
                .WithMessage ((command, code) =>
                    localizer.Format (
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.Course],
                        code));

            // 4️ Kiểm tra CourseName duy nhất
            RuleFor (x => x.CourseModel.CourseName)
                .MustAsync (async (name, cancellation) =>
                    !await dbContext.Courses.AnyAsync (c => c.CourseName == name && !c.IsDeleted, cancellation))
                .WithMessage ((command, name) =>
                    localizer.Format (
                        LocalizationKey.ModelNameAlreadyExists,
                        localizer[EntityName.Course],
                        name));

            // 5️ MidExamIds: GUID hợp lệ và tồn tại trong CategoryType.Skill
            RuleFor (x => x.CourseModel.MidExamIds)
                .MustAsync (async (ids, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace (ids)) return true;

                    var guidList = ids.Split (',')
                        .Select (s => Guid.TryParse (s.Trim ( ), out var g) ? g : Guid.Empty)
                        .Where (g => g != Guid.Empty)
                        .ToList ( );

                    if (!guidList.Any ( )) return false;

                    // Lấy danh sách CategoryType.Skill từ DB
                    var skillCategories = await dbContext.Categories
                        .Where (c => c.CategoryType == (int)CategoryType.Skill)
                        .Select (c => c.Id)
                        .ToListAsync (cancellation);

                    // Kiểm tra tất cả GUID có tồn tại trong skillCategories
                    return guidList.All (g => skillCategories.Contains (g));
                })
                .WithMessage (localizer["MidExamIdsMustBeValidCategorySkill"]);

            // 6️ FinalExamIds: GUID hợp lệ và tồn tại trong CategoryType.Skill
            RuleFor (x => x.CourseModel.FinalExamIds)
                .MustAsync (async (ids, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace (ids)) return true;

                    var guidList = ids.Split (',')
                        .Select (s => Guid.TryParse (s.Trim ( ), out var g) ? g : Guid.Empty)
                        .Where (g => g != Guid.Empty)
                        .ToList ( );

                    if (!guidList.Any ( )) return false;

                    var skillCategories = await dbContext.Categories
                        .Where (c => c.CategoryType == (int)CategoryType.Skill)
                        .Select (c => c.Id)
                        .ToListAsync (cancellation);

                    return guidList.All (g => skillCategories.Contains (g));
                })
                .WithMessage (localizer["FinalExamIdsMustBeValidCategorySkill"]);

        }
    }
}
