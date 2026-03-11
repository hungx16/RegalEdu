using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Promotion.Commands;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Promotion.Validators
{
    public class DeleteListPromotionCommandValidator : AbstractValidator<DeleteListPromotionCommand>
    {
        public DeleteListPromotionCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["PromotionDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Promotion));
        }
    }
}
