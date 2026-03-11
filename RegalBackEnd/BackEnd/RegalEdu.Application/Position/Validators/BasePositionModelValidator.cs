using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Validators
{
    public class BasePositionModelValidator : AbstractValidator<PositionModel>
    {
        public BasePositionModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.PositionCode)
                .NotEmpty ( ).WithMessage (localizer["PositionCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("PositionCodeMaxLength", 10));

            RuleFor (x => x.PositionName)
                .NotEmpty ( ).WithMessage (localizer["PositionNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("PositionNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("PositionDescriptionMaxLength", 1000));
        }
    }
}
