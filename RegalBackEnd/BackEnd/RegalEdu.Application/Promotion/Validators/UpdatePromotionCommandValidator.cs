using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Promotion.Commands;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Promotion.Validators
{
    public class UpdatePromotionCommandValidator : AbstractValidator<UpdatePromotionCommand>
    {
        public UpdatePromotionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.PromotionModel).SetValidator(new BasePromotionModelValidator(localizer));

            RuleFor(x => x.PromotionModel)
                .MustAsync(async (m, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(m.Code)) return true;
                    return !await db.Promotions.AnyAsync(p => p.Code == m.Code && p.Id != m.Id && !p.IsDeleted, ct);
                })
                .WithMessage((cmd, m) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists, EntityName.Promotion, m.Code ?? string.Empty));
        }
    }
}
