using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.WorkBoardTeacher.Commands;

namespace RegalEdu.Application.WorkBoardTeacher.Validators
{
    public class ConfirmWorkBoardTeacherCommandValidator : AbstractValidator<ConfirmWorkBoardTeacherCommand>
    {
        public ConfirmWorkBoardTeacherCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ConfirmedBy)
                .NotEmpty().WithMessage(localizer["ConfirmedByRequired"])
                .MaximumLength(50).WithMessage(localizer.Format("ConfirmedByMaxLength", 50));
        }
    }
}