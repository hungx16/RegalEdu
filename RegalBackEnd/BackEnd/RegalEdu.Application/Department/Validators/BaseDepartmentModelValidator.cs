using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Validators
{
    public class BaseDepartmentModelValidator : AbstractValidator<DepartmentModel>
    {
        public BaseDepartmentModelValidator(ILocalizationService localizer)
        {
            string start = AutoCodeConfig.Get (AutoCodeType.Department).Prefix;
            int length = AutoCodeConfig.Get (AutoCodeType.Department).Length;
            RuleFor (x => x.DepartmentCode)
                .NotEmpty ( ).WithMessage (localizer["DepartmentCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("DepartmentCodeMaxLength", 10))
                .Matches ($"^{start}\\d{{{length}}}$")
                .WithMessage (localizer.Format ("DepartmentCodeInvalidFormat", start, length));


            RuleFor (x => x.DepartmentName)
                .NotEmpty ( ).WithMessage (localizer["DepartmentNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("DepartmentNameMaxLength", 200));

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("DepartmentDescriptionMaxLength", 1000));

            RuleFor (x => x.DivisionId)
                .NotEmpty ( ).WithMessage (localizer["DivisionIdRequired"]);
        }
    }
}
