using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.EvaluateTeacher.Commands;

namespace RegalEdu.Application.EvaluateTeacher.Validators
{
    public class RespondEvaluateTeacherCommandValidator : AbstractValidator<RespondEvaluateTeacherCommand>
    {
        public RespondEvaluateTeacherCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ResponseContent)
                .NotEmpty().WithMessage(localizer["ResponseContentRequired"])
                .MaximumLength(1000).WithMessage(localizer.Format("ResponseContentMaxLength", 1000));
        }
    }
}