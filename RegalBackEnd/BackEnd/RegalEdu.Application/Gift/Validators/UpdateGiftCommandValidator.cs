using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Gift.Validators
{
    public class UpdateGiftCommandValidator : AbstractValidator<UpdateGiftCommand>
    {
        public UpdateGiftCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.GiftModel)
                .SetValidator(new BaseGiftModelValidator(localizer));

            RuleFor(x => x.GiftModel)
                .MustAsync(async (model, cancellation) =>
                {
                    return !await dbContext.Gift
                        .AnyAsync(g => g.Name == model.Name && g.Id != model.Id && !g.IsDeleted, cancellation);
                })
                .WithMessage((command, model) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists, EntityName.Gift, model.Name ?? string.Empty));
        }
    }
}
