using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Validators
{
    public class BaseLearningRoadMapModelValidator : AbstractValidator<LearningRoadMapModel>
    {
        public BaseLearningRoadMapModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.LearningRoadMapCode)
                .NotEmpty ( ).WithMessage (localizer["LearningRoadMapCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("LearningRoadMapCodeMaxLength", 10))
                .Must (code => !code.Contains (" "))
                .WithMessage (localizer["LearningRoadMapCodeNoSpaces"]);

            RuleFor (x => x.LearningRoadMapName)
                .NotEmpty ( ).WithMessage (localizer["LearningRoadMapNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("LearningRoadMapNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("LearningRoadMapDescriptionMaxLength", 1000));

            RuleFor (x => x.Order)
                .InclusiveBetween (1, 100)
                .WithMessage (localizer.Format (LocalizationKey.InvalidOrderRange, 1, 100));
        }
    }
}