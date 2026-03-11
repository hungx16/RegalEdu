using FluentValidation;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Validators
{
    public class BaseCompanyModelValidator : AbstractValidator<CompanyModel>
    {
        public BaseCompanyModelValidator(ILocalizationService localizer)
        {
            string start = AutoCodeConfig.Get (AutoCodeType.Company).Prefix;
            int length = AutoCodeConfig.Get (AutoCodeType.Company).Length;

            RuleFor (x => x.CompanyCode)
                .NotEmpty ( ).WithMessage (localizer["CompanyCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("CompanyCodeMaxLength", 10))
                .Matches ($"^{start}\\d{{{length}}}$")
                .WithMessage (localizer.Format ("CompanyCodeInvalidFormat", start, length));

            RuleFor (x => x.CompanyName)
                .NotEmpty ( ).WithMessage (localizer["CompanyNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("CompanyNameMaxLength", 200));

            RuleFor (x => x.CompanyAddress)
                .MaximumLength (1000).WithMessage (localizer.Format ("CompanyAddressMaxLength", 1000));

            RuleFor (x => x.CompanyPhone)
                .MaximumLength (20).WithMessage (localizer.Format ("CompanyPhoneMaxLength", 20));

            RuleFor (x => x.ProvinceCode)
                .NotEmpty ( ).WithMessage (localizer["ProvinceCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("ProvinceCodeMaxLength", 10))
                .MustAsync (async (command, provinceCode, cancellation) =>
                {
                    var provinces = await ProvinceFileHelper.LoadProvincesAsync ( );
                    return provinces.Any (p => p.ProvinceCode == provinceCode);
                })
                .WithMessage ((command, provinceCode) => localizer.Format ("ProvinceCodeInvalid", provinceCode));
        }
    }
}
