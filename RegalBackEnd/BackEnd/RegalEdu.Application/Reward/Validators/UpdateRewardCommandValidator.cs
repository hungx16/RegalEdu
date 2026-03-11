using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Reward.Commands;

namespace RegalEdu.Application.Reward.Validators
{
    public class UpdateRewardCommandValidator : AbstractValidator<UpdateRewardCommand>
    {
        public UpdateRewardCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.RewardModel).NotNull();
            RuleFor(x => x.RewardModel.Id).NotNull().WithMessage(localizer["IdRequired"]);
            RuleFor(x => x.RewardModel.Name).NotEmpty().WithMessage(localizer["NameRequired"]).MaximumLength(200);
        }
    }
}
