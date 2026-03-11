using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CouponIssue.Commands;
using RegalEdu.Application.CouponType.Commands;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponIssue.Validators
{
    public class DeleteListCouponIssueCommandValidator : AbstractValidator<DeleteListCouponIssueCommand>
    {
        public DeleteListCouponIssueCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["CouponIssueDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.CouponIssue));
        }
    }
}