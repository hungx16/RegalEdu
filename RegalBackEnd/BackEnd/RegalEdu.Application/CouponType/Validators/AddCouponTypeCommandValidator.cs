using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CouponType.Commands;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponType.Validators
{
    public class AddCouponTypeCommandValidator : AbstractValidator<AddCouponTypeCommand>
    {
        public AddCouponTypeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.CouponTypeModel)
                .SetValidator(new BaseCouponTypeModelValidator(localizer));

            RuleFor(x => x.CouponTypeModel.Code)
                .MustAsync(async (code, cancellation) =>
                {
                    return !await dbContext.CouponType.AnyAsync(ct => ct.Code == code && !ct.IsDeleted, cancellation);
                })
                .WithMessage((command, code) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists, EntityName.CouponType, code));
        }
    }
}
