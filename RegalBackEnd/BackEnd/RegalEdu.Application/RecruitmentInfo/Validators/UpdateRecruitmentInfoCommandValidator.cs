using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.RecruitmentInfo.Commands;

namespace RegalEdu.Application.RecruitmentInfo.Validators
{
    public class UpdateRecruitmentInfoCommandValidator : AbstractValidator<UpdateRecruitmentInfoCommand>
    {
        public UpdateRecruitmentInfoCommandValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.RecruitmentInfoModel).SetValidator (new BaseRecruitmentInfoModelValidator (localizer));
        }
    }
}
