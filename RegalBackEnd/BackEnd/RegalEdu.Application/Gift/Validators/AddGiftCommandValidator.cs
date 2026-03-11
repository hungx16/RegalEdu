using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Gift.Validators
{
    public class AddGiftCommandValidator : AbstractValidator<AddGiftCommand>
    {
        public AddGiftCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.GiftModel)
                .SetValidator(new BaseGiftModelValidator(localizer));

            RuleFor(x => x.GiftModel.Name)
                .MustAsync(async (name, cancellation) =>
                {
                    return !await dbContext.Gift
                        .AnyAsync(g => g.Name == name && !g.IsDeleted, cancellation);
                })
                .WithMessage((command, name) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists, EntityName.Gift, name));
        }
    }
}