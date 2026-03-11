using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Validators
{
    public class BaseWorkingTimeConfigurationModelValidator : AbstractValidator<WorkingTimeConfigurationModel>
    {
        public BaseWorkingTimeConfigurationModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.NameConfiguration)
                .NotEmpty ( ).WithMessage (localizer["WTCNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("WTCNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("WTCDescriptionMaxLength", 1000));
        }
    }
}
