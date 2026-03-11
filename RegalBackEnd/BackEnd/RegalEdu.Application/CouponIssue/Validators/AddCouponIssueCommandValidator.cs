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
    public class AddCouponIssueCommandValidator : AbstractValidator<AddCouponIssueCommand>
    {
        public AddCouponIssueCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.CouponIssueModel)
                .SetValidator(new BaseCouponIssueModelValidator(localizer));

            // (Tuỳ chọn) Có thể enforce: không trùng IssueType trong cùng ngày phát hành nếu là rule của domain:
            //RuleFor(x => new { x.CouponIssueModel.IssueType, x.CouponIssueModel.IssueDate })
            //    .MustAsync(async (m, ct) =>
            //    {
            //        if (m.IssueType == null || m.IssueDate == null) return true;
            //        return !await db.CouponIssues.AnyAsync(ci =>
            //            ci.IssueType == m.IssueType && ci.IssueDate == m.IssueDate && !ci.IsDeleted, ct);
            //    })
            //    .WithMessage(localizer["CouponIssueDuplicated"]);
        }
    }
}
