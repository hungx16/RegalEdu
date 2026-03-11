using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CustomerReward.Commands;

namespace RegalEdu.Application.CustomerReward.Validators
{
    public class ConfirmAcceptanceCustomerRewardCommandValidator : AbstractValidator<ConfirmAcceptanceCustomerRewardCommand>
    {
        public ConfirmAcceptanceCustomerRewardCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer["IdRequired"]);
            RuleFor(x => x.ConfirmedBy).NotEmpty().WithMessage(localizer["ConfirmedByRequired"]);
        }
    }
}
