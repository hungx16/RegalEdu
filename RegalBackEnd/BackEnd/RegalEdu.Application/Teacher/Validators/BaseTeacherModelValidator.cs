using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Validators
{
    public class BaseTeacherModelValidator : AbstractValidator<TeacherModel>
    {
        public BaseTeacherModelValidator(ILocalizationService localizer)
        {
            string start = AutoCodeConfig.Get (AutoCodeType.Teacher).Prefix;
            int length = AutoCodeConfig.Get (AutoCodeType.Teacher).Length;

            //RuleFor(x => x.TeacherCode)
            //    .NotEmpty().WithMessage(localizer["TeacherCodeRequired"])
            //    .MaximumLength(10).WithMessage(localizer.Format("TeacherCodeMaxLength", 10))
            //    .Matches($"^{start}\\d{{{length}}}$")
            //    .WithMessage(localizer.Format("TeacherCodeInvalidFormat", start, length));

            //RuleFor(x => x.TeacherName)
            //    .NotEmpty().WithMessage(localizer["TeacherNameRequired"])
            //    .MaximumLength(200).WithMessage(localizer.Format("TeacherNameMaxLength", 200));
        }
    }
}