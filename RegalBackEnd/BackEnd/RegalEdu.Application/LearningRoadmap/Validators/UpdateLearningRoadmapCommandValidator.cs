using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.LearningRoadMap.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LearningRoadMap.Validators
{
    public class UpdateLearningRoadMapCommandValidator : AbstractValidator<UpdateLearningRoadMapCommand>
    {
        public UpdateLearningRoadMapCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1. Validate model con (các rule chung trong BaseLearningRoadMapModelValidator)
            RuleFor(x => x.LearningRoadMapModel)
                .SetValidator(new BaseLearningRoadMapModelValidator(localizer));

            // 2. Kiểm tra AgeGroupId tồn tại trong Category (không bị xóa và đúng loại AgeGroup)
            RuleFor(x => x.LearningRoadMapModel.AgeGrId)
                .MustAsync(async (ageGroupId, cancellation) =>
                {
                    return await dbContext.Categories.AsNoTracking().AnyAsync(
                        c => c.Id == ageGroupId
                          && c.CategoryType == (byte)CategoryType.AgeGroup,                         
                        cancellation);
                })
                .WithMessage(localizer.Format(LocalizationKey.InvalidCategoryType, EntityName.Category, CategoryType.AgeGroup));

            // 3. Kiểm tra mã chương trình là duy nhất (trừ chính bản ghi đang sửa)
            RuleFor(x => x.LearningRoadMapModel.LearningRoadMapCode)
                .MustAsync(async (command, learningRoadMapCode, cancellation) =>
                {
                    return !await dbContext.LearningRoadMaps.AsNoTracking().AnyAsync(
                        d => d.LearningRoadMapCode == learningRoadMapCode
                          && d.Id != command.LearningRoadMapModel.Id
                          && !d.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, learningRoadMapCode) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.LearningRoadMap], learningRoadMapCode));

            // 4. Kiểm tra tên chương trình là duy nhất (trừ chính bản ghi đang sửa)
            RuleFor(x => x.LearningRoadMapModel.LearningRoadMapName)
                .MustAsync(async (command, learningRoadMapName, cancellation) =>
                {
                    return !await dbContext.LearningRoadMaps.AsNoTracking().AnyAsync(
                        d => d.LearningRoadMapName == learningRoadMapName
                          && d.Id != command.LearningRoadMapModel.Id
                          && !d.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, learningRoadMapName) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists,
                        localizer[EntityName.LearningRoadMap], learningRoadMapName));
        }
    }
}
