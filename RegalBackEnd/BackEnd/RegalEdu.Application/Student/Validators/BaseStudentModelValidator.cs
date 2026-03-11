using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Validators
{
    public class BaseStudentModelValidator : AbstractValidator<StudentModel>
    {
        public BaseStudentModelValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(localizer["StudentFullNameRequired"])
                .MaximumLength(200).WithMessage(localizer.Format("StudentFullNameMaxLength", 200));

            RuleFor(x => x.StudentCode)
                .MaximumLength(50).WithMessage(localizer.Format("StudentCodeMaxLength", 50));

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(localizer["StudentEmailInvalid"]);
        }
    }
}
