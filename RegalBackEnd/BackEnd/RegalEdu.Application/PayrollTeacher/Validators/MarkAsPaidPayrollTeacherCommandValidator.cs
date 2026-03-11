using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.PayrollTeacher.Commands;

namespace RegalEdu.Application.PayrollTeacher.Validators
{
    public class MarkAsPaidPayrollTeacherCommandValidator : AbstractValidator<MarkAsPaidPayrollTeacherCommand>
    {
        public MarkAsPaidPayrollTeacherCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.PaidDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(localizer["PaidDateCannotBeFuture"])
                .When(x => x.PaidDate.HasValue);
        }
    }
}