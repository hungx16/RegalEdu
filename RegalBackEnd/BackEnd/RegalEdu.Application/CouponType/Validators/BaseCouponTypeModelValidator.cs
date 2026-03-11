using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Validators
{
    public class BaseCouponTypeModelValidator : AbstractValidator<CouponTypeModel>
    {
        public BaseCouponTypeModelValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["CouponTypeNameRequired"])
                .MaximumLength(200).WithMessage(localizer.Format("CouponTypeNameMaxLength", 200));

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(localizer["CouponTypeCodeRequired"])
                .MaximumLength(50).WithMessage(localizer.Format("CouponTypeCodeMaxLength", 50));
        }
    }
}
