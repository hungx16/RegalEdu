using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Validators
{
    public class BaseRegisterStudyModelValidator : AbstractValidator<RegisterStudyModel>
    {
        public BaseRegisterStudyModelValidator(ILocalizationService localizer)
        {
            //RuleFor(x => x.Code)
            //    .NotEmpty().WithMessage(localizer["RegisterStudyCodeRequired"])
            //    .MaximumLength(20).WithMessage(localizer.Format("RegisterStudyCodeMaxLength", 20));

            //RuleFor(x => x.StudentId)
            //    .NotNull().WithMessage(localizer["RegisterStudyStudentRequired"]);

            RuleFor(x => x.TotalAmount)
                .Must(v => v == null || v >= 0)
                .WithMessage(localizer["RegisterStudyTotalAmountNonNegative"]);
        }
    }
}
