using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.RecruitmentInfo.Commands;

namespace RegalEdu.Application.RecruitmentInfo.Validators
{
    public class AddRecruitmentInfoCommandValidator : AbstractValidator<AddRecruitmentInfoCommand>
    {
        public AddRecruitmentInfoCommandValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.RecruitmentInfoModel).SetValidator (new BaseRecruitmentInfoModelValidator (localizer));
        }
    }
}
