using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Gift.Validators
{
    public class DeleteListGiftCommandValidator : AbstractValidator<DeleteListGiftCommand>
    {
        public DeleteListGiftCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["GiftDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Gift));
        }
    }
}
