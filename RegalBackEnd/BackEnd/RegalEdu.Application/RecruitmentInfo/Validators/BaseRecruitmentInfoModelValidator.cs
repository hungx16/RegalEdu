using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Validators
{
    public class BaseRecruitmentInfoModelValidator : AbstractValidator<RecruitmentInfoModel>
    {
        public BaseRecruitmentInfoModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.RecruitmentInfoName)
                .NotEmpty ( ).WithMessage (localizer["RecruitmentInfoNameRequired"])
                .MaximumLength (255).WithMessage (localizer.Format ("RecruitmentInfoNameMaxLength", 255));

            RuleFor (x => x.Experience)
                .MaximumLength (100).WithMessage (localizer.Format ("ExperienceMaxLength", 100));

            RuleFor (x => x.Salary)
                .GreaterThanOrEqualTo (0).WithMessage (localizer["SalaryMustBePositive"]);

            RuleFor (x => x.Position)
                .NotEmpty ( ).WithMessage (localizer["PositionRequired"]);

            RuleFor (x => x.ProvinceCode)
                .NotEmpty ( ).WithMessage (localizer["ProvinceRequired"]);
        }
    }
}
