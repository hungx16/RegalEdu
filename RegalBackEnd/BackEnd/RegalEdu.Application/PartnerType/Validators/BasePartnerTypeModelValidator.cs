using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PartnerType.Validators
{
    public class BasePartnerTypeModelValidator : AbstractValidator<PartnerTypeModel>
    {
        public BasePartnerTypeModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.PartnerTypeCode)
                .NotEmpty ( ).WithMessage (localizer["PartnerTypeCodeRequired"])
                .MaximumLength (50).WithMessage (localizer.Format ("PartnerTypeCodeMaxLength", 50));


            RuleFor (x => x.Description)
                .MaximumLength (300).WithMessage (localizer.Format ("PartnerTypeDescriptionMaxLength", 300));
        }
    }
}