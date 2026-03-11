using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Student.Commands;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Validators
{
    public class DeleteListStudentCommandValidator : AbstractValidator<DeleteListStudentCommand>
    {
        public DeleteListStudentCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ListIds)
                .NotNull().WithMessage(localizer["StudentDeleteListRequired"])
                .Must(ids => ids != null && ids.Any())
                .WithMessage(localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Student));
        }
    }
}
