using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Validators
{
    public class BaseCouponIssueModelValidator : AbstractValidator<CouponIssueModel>
    {
        public BaseCouponIssueModelValidator(ILocalizationService localizer)
        {
            //RuleFor(x => x.IssueType)
            //    .NotEmpty().WithMessage(localizer["CouponIssueTypeRequired"])
            //    .MaximumLength(100).WithMessage(localizer.Format("CouponIssueTypeMaxLength", 100));

            //RuleFor(x => x.Quantity)
            //    .NotNull().WithMessage(localizer["CouponIssueQuantityRequired"])
            //    .GreaterThan(0).WithMessage(localizer["CouponIssueQuantityPositive"]);

            RuleFor(x => x.IssueDate)
                .NotNull().WithMessage(localizer["CouponIssueDateRequired"]);
        }
    }
}
