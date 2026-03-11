using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PromotionGroup.Validators
{
    public class UpdatePromotionGroupCommandValidator : AbstractValidator<UpdatePromotionGroupCommand>
    {
        public UpdatePromotionGroupCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.PromotionGroupModel)
                .SetValidator(new BasePromotionGroupModelValidator(localizer));

            RuleFor(x => x.PromotionGroupModel)
                .MustAsync(async (model, cancellation) =>
                {
                    return !await dbContext.PromotionGroup
                        .AnyAsync(pg => pg.Name == model.Name && pg.Id != model.Id && !pg.IsDeleted, cancellation);
                })
                .WithMessage((command, model) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists, EntityName.PromotionGroup, model.Name ?? string.Empty));
        }
    }
}