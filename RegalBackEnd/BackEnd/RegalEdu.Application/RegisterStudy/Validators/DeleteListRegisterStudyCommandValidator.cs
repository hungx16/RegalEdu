using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Application.RegisterStudy.Commands;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Validators
{
    public class DeleteListRegisterStudyCommandValidator : AbstractValidator<DeleteListRegisterStudyCommand>
    {
        public DeleteListRegisterStudyCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["RegisterStudyDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.RegisterStudy));
        }
    }
}
