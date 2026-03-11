using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Validators
{
    public class BaseRegionModelValidator : AbstractValidator<RegionModel>
    {
        public BaseRegionModelValidator(ILocalizationService localizer)
        {
            string start = AutoCodeConfig.Get (AutoCodeType.Region).Prefix;
            int length = AutoCodeConfig.Get (AutoCodeType.Region).Length;

            RuleFor (x => x.RegionCode)
                .NotEmpty ( ).WithMessage (localizer["RegionCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("RegionCodeMaxLength", 10))
                .Matches ($"^{start}\\d{{{length}}}$")
                .WithMessage (localizer.Format ("RegionCodeInvalidFormat", start, length));

            RuleFor (x => x.RegionName)
                .NotEmpty ( ).WithMessage (localizer["RegionNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("RegionNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("RegionDescriptionMaxLength", 1000));
        }
    }
}
