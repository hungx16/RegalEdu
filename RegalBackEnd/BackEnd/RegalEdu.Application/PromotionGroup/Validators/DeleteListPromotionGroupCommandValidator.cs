using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PromotionGroup.Validators
{
    public class DeleteListPromotionGroupCommandValidator : AbstractValidator<DeleteListPromotionGroupCommand>
    {
        public DeleteListPromotionGroupCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["PromotionGroupDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.PromotionGroup));
        }
    }
}