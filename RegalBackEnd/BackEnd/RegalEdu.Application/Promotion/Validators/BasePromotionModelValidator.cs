using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Validators
{
    public class BasePromotionModelValidator : AbstractValidator<PromotionModel>
    {
        public BasePromotionModelValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["PromotionNameRequired"])
                .MaximumLength(200).WithMessage(localizer.Format("PromotionNameMaxLength", 200));

            RuleFor(x => x.StartDate).NotEmpty().WithMessage(localizer["PromotionStartDateRequired"]);

            RuleFor(x => x.EndDate)
                .Must((m, end) => end >= m.StartDate)
                .WithMessage(localizer["PromotionEndDateNotBeforeStartDate"]);
        }
    }

}
