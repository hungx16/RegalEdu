using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Validators
{
    public class BasePromotionGroupModelValidator : AbstractValidator<PromotionGroupModel>
    {
        public BasePromotionGroupModelValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["PromotionGroupNameRequired"])
                .MaximumLength(200).WithMessage(localizer.Format("PromotionGroupNameMaxLength", 200));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage(localizer.Format("PromotionGroupDescriptionMaxLength", 1000));
        }
    }
}
