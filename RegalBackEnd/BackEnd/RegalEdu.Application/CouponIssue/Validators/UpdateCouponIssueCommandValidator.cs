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
    public class UpdateCouponIssueCommandValidator : AbstractValidator<UpdateCouponIssueCommand>
    {
        public UpdateCouponIssueCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.CouponIssueModel)
                .SetValidator(new BaseCouponIssueModelValidator(localizer));

            RuleFor(x => x.CouponIssueModel)
                .MustAsync(async (model, ct) =>
                {
                    if (model.IssueType == null || model.IssueDate == null) return true;
                    return !await db.CouponIssues.AnyAsync(ci =>
                        ci.IssueType == model.IssueType &&
                        ci.IssueDate == model.IssueDate &&
                        ci.Id != model.Id &&
                        !ci.IsDeleted, ct);
                })
                .WithMessage(localizer["CouponIssueDuplicated"]);
        }
    }
}