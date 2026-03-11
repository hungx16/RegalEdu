using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PromotionGroup.Validators
{
    public class AddPromotionGroupCommandValidator : AbstractValidator<AddPromotionGroupCommand>
    {
        public AddPromotionGroupCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.PromotionGroupModel)
                .SetValidator(new BasePromotionGroupModelValidator(localizer));

            RuleFor(x => x.PromotionGroupModel.Name)
                .MustAsync(async (name, cancellation) =>
                {
                    return !await dbContext.PromotionGroup
                        .AnyAsync(pg => pg.Name == name && !pg.IsDeleted, cancellation);
                })
                .WithMessage((command, name) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists, EntityName.PromotionGroup, name));
        }
    }
}
