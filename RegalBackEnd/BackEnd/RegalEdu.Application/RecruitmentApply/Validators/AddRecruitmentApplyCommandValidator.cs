using FluentValidation;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.RecruitmentApply.Validators
{
    public class AddRecruitmentApplyCommandValidator : AbstractValidator<Commands.AddRecruitmentApplyCommand>
    {
        public AddRecruitmentApplyCommandValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.RecruitmentApplyModel)
                .SetValidator (new BaseRecruitmentApplyModelValidator (localizer));
        }
    }
}
