using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.LearningRoadMap.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LearningRoadMap.Validators
{
    public class AddLearningRoadMapValidator : AbstractValidator<AddLearningRoadMapCommand>
    {
        public AddLearningRoadMapValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {

            // 1. Kiểm tra các rule cơ bản của model (rỗng, độ dài, format...)
            //    => không đụng đến DB, chạy nhanh, nên đặt đầu tiên
            RuleFor(x => x.LearningRoadMapModel)
                .SetValidator(new BaseLearningRoadMapModelValidator(localizer));


            // 2. Kiểm tra AgeGrId có tồn tại trong bảng Category với CategoryType = AgeGroup
            //    => đảm bảo dữ liệu tham chiếu hợp lệ trước khi check các ràng buộc phức tạp khác
            RuleFor(x => x.LearningRoadMapModel.AgeGrId)
                .MustAsync(async (ageGrId, cancellation) =>
                {
                    return await dbContext.Categories.AnyAsync(
                        c => c.Id == ageGrId
                             && c.CategoryType == (byte)CategoryType.AgeGroup
                             && !c.IsDeleted,
                        cancellation);
                })
                .WithMessage(localizer.Format(LocalizationKey.AgeGrIdNotFound));

            // 3. Kiểm tra tên LearningRoadMapCode có duy nhất không
            //    => đặt cuối cùng vì phải query DB so sánh uniqueness
            RuleFor(x => x.LearningRoadMapModel.LearningRoadMapCode)
                .MustAsync(async (code, cancellation) =>
                {
                    return !await dbContext.LearningRoadMaps.AnyAsync(
                        d => d.LearningRoadMapCode == code && !d.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, code) => localizer.Format(
                    LocalizationKey.ModelCodeAlreadyExists,
                    localizer[EntityName.LearningRoadMap],
                    code));

            // 4. Kiểm tra tên LearningRoadMapName có duy nhất không
            //    => đặt cuối cùng vì phải query DB so sánh uniqueness
            RuleFor(x => x.LearningRoadMapModel.LearningRoadMapName)
                .MustAsync(async (name, cancellation) =>
                {
                    return !await dbContext.LearningRoadMaps.AnyAsync(
                        d => d.LearningRoadMapName == name && !d.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, name) => localizer.Format(
                    LocalizationKey.ModelNameAlreadyExists,
                    localizer[EntityName.LearningRoadMap],                    
                    name));

        }
    }
}
