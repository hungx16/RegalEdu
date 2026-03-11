using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CustomerReward.Commands;

namespace RegalEdu.Application.CustomerReward.Validators
{
    public class UpdateCustomerRewardCommandValidator : AbstractValidator<UpdateCustomerRewardCommand>
    {
        public UpdateCustomerRewardCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.CustomerRewardModel).NotNull();
            RuleFor(x => x.CustomerRewardModel.Id).NotNull().WithMessage(localizer["IdRequired"]);
            RuleFor(x => x.CustomerRewardModel.FullName).NotEmpty().WithMessage(localizer["FullNameRequired"]).MaximumLength(150);
            RuleFor(x => x.CustomerRewardModel.Phone).NotEmpty().WithMessage(localizer["PhoneRequired"]).MaximumLength(15);
        }
    }
}
