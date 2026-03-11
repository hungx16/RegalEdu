using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CustomerReward.Commands;

namespace RegalEdu.Application.CustomerReward.Validators
{
    public class CreateCustomerRewardCommandValidator : AbstractValidator<CreateCustomerRewardCommand>
    {
        public CreateCustomerRewardCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.CustomerRewardModel).NotNull();
            RuleFor(x => x.CustomerRewardModel.WonDate).NotEmpty().WithMessage(localizer["WonDateRequired"]);
            RuleFor(x => x.CustomerRewardModel.Prize).NotEmpty().WithMessage(localizer["PrizeRequired"]).MaximumLength(200);
            RuleFor(x => x.CustomerRewardModel.Phone).NotEmpty().WithMessage(localizer["PhoneRequired"]).MaximumLength(15);
            RuleFor(x => x.CustomerRewardModel.FullName).NotEmpty().WithMessage(localizer["FullNameRequired"]).MaximumLength(150);
        }
    }
}
