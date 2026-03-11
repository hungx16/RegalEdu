using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Validators
{
    public class BaseItemModelValidator : AbstractValidator<ItemModel>
    {
        public BaseItemModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.ItemCode)
                .NotEmpty ( ).WithMessage (localizer["ItemCodeRequired"]);

            RuleFor (x => x.ItemName)
                .NotEmpty ( ).WithMessage (localizer["ItemNameRequired"]);

            RuleFor (x => x.Price)
                .GreaterThanOrEqualTo (0m).WithMessage (localizer["ItemPriceNonNegative"]);

            RuleFor (x => x.Quantity)
                .GreaterThanOrEqualTo (0).WithMessage (localizer["ItemQuantityNonNegative"]);
        }
    }
}
