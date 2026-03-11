using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Degree.Validators
{
    public class BaseDegreeModelValidator : AbstractValidator<DegreeModel>
    {
        public BaseDegreeModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.DegreeName)
                .NotEmpty ( ).WithMessage (localizer["DegreeNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("DegreeNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("DegreeDescriptionMaxLength", 1000));
        }
    }
}
