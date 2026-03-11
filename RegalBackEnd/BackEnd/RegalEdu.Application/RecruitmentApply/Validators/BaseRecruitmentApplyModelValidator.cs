using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentApply.Validators
{
    public class BaseRecruitmentApplyModelValidator : AbstractValidator<RecruitmentApplyModel>
    {
        public BaseRecruitmentApplyModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.CandidateName)
                .NotEmpty ( ).WithMessage (localizer["CandidateNameRequired"])
                .MaximumLength (255).WithMessage (localizer.Format ("CandidateNameMaxLength", 255));

            RuleFor (x => x.CandidateEmail)
                .NotEmpty ( ).WithMessage (localizer["CandidateEmailRequired"])
                .EmailAddress ( ).WithMessage (localizer["CandidateEmailInvalid"])
                .MaximumLength (255).WithMessage (localizer.Format ("CandidateEmailMaxLength", 255));

            RuleFor (x => x.CandidatePhone)
                .NotEmpty ( ).WithMessage (localizer["CandidatePhoneRequired"])
                .MaximumLength (20).WithMessage (localizer.Format ("CandidatePhoneMaxLength", 20));

            RuleFor (x => x.CandidateCV)
                .MaximumLength (1000).WithMessage (localizer.Format ("CandidateCVMaxLength", 1000))
                .When (x => !string.IsNullOrWhiteSpace (x.CandidateCV));

            RuleFor (x => x.RecruitmentInfoId)
                .NotEmpty ( ).WithMessage (localizer["RecruitmentInfoIdRequired"]);
        }
    }
}
