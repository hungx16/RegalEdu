using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.EvaluateTeacher.Commands;

namespace RegalEdu.Application.EvaluateTeacher.Validators
{
    public class AddEvaluateTeacherCommandValidator : AbstractValidator<AddEvaluateTeacherCommand>
    {
        public AddEvaluateTeacherCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.EvaluateTeacherModel.TeacherId)
                .NotEmpty().WithMessage(localizer["TeacherRequired"]);

            When(x => x.EvaluateTeacherModel.StarRating.HasValue, () =>
            {
                RuleFor(x => x.EvaluateTeacherModel.StarRating)
                    .InclusiveBetween(0.0, 5.0).WithMessage(localizer.Format("StarRatingRange", 0, 5));
            });

            RuleFor(x => x.EvaluateTeacherModel.ResponseContent)
                .MaximumLength(1000).WithMessage(localizer.Format("ResponseContentMaxLength", 1000));

            RuleFor(x => x.EvaluateTeacherModel.EvaluateName)
                .MaximumLength(1000).WithMessage(localizer.Format("EvaluateNameMaxLength", 1000));

            RuleFor(x => x.EvaluateTeacherModel.EvaluateNick)
                .MaximumLength(1000).WithMessage(localizer.Format("EvaluateNickMaxLength", 1000));
        }
    }
}
