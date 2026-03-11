using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Student.Commands;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Validators
{
    public class AddStudentCommandValidator : AbstractValidator<AddStudentCommand>
    {
        public AddStudentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.StudentModel).SetValidator(new BaseStudentModelValidator(localizer));

            RuleFor(x => x.StudentModel.StudentCode)
                .MustAsync(async (code, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(code)) return true;
                    return !await db.Students.AnyAsync(s => s.StudentCode == code && !s.IsDeleted, ct);
                })
                .WithMessage((cmd, code) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists, EntityName.Student, code ?? string.Empty));
        }
    }
}
