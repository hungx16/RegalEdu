using FluentValidation;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.RecruitmentApply.Validators
{
    public class UpdateRecruitmentApplyCommandValidator : AbstractValidator<Commands.UpdateRecruitmentApplyCommand>
    {
        public UpdateRecruitmentApplyCommandValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.RecruitmentApplyModel)
                .SetValidator (new BaseRecruitmentApplyModelValidator (localizer));
        }
    }
}
