using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CouponType.Commands;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponType.Validators
{
    public class UpdateCouponTypeCommandValidator : AbstractValidator<UpdateCouponTypeCommand>
    {
        public UpdateCouponTypeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.CouponTypeModel)
                .SetValidator(new BaseCouponTypeModelValidator(localizer));

            RuleFor(x => x.CouponTypeModel)
                .MustAsync(async (model, cancellation) =>
                {
                    return !await dbContext.CouponType
                        .AnyAsync(ct => ct.Code == model.Code && ct.Id != model.Id && !ct.IsDeleted, cancellation);
                })
                .WithMessage((command, model) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists, EntityName.CouponType, model.Code ?? string.Empty));
        }
    }
}