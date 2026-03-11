using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Promotion.Commands;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Validators
{
    public class AddPromotionCommandValidator : AbstractValidator<AddPromotionCommand>
    {
        public AddPromotionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.PromotionModel).SetValidator(new BasePromotionModelValidator(localizer));

            // Code (nếu dùng) phải duy nhất
            RuleFor(x => x.PromotionModel.Code)
                .MustAsync(async (code, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(code)) return true;
                    return !await db.Promotions.AnyAsync(p => p.Code == code && !p.IsDeleted, ct);
                })
                .WithMessage((cmd, code) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists, EntityName.Promotion, code ?? string.Empty));
        }
    }
}
