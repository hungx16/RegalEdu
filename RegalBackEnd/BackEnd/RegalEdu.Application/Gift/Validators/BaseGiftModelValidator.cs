using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
namespace RegalEdu.Application.Gift.Validators
{
    public class BaseGiftModelValidator : AbstractValidator<GiftModel>
{
    public BaseGiftModelValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(localizer["GiftNameRequired"])
            .MaximumLength(200).WithMessage(localizer.Format("GiftNameMaxLength", 200));

        RuleFor(x => x.Prices)
            .GreaterThan(0).WithMessage(localizer["GiftPriceMustBePositive"]);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(localizer.Format("GiftDescriptionMaxLength", 1000));
    }
}
}
