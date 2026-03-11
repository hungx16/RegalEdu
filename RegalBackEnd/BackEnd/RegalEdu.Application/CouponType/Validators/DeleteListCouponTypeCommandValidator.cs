using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.CouponType.Commands;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.CouponType.Validators
{
    public class DeleteListCouponTypeCommandValidator : AbstractValidator<DeleteListCouponTypeCommand>
    {
        public DeleteListCouponTypeCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["CouponTypeDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.CouponType));
        }
    }
}