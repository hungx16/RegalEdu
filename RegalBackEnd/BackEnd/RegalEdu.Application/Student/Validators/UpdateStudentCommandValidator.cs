using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Student.Commands;

namespace RegalEdu.Application.Student.Validators
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.StudentModel)
                .SetValidator (new BaseStudentModelValidator (localizer));

            //RuleFor (x => x.StudentModel.StudentCode)
            //    .MustAsync (async (command, code, cancellation) =>
            //        !await dbContext.Students.AnyAsync (r => r.StudentCode == code && r.Id != command.StudentModel.Id && !r.IsDeleted, cancellation))
            //    .WithMessage ((command, code) => localizer.Format ("ModelCodeAlreadyExists", localizer["Student"], code));

            RuleFor (x => x.StudentModel.FullName)
                .MustAsync (async (command, name, cancellation) =>
                    !await dbContext.Students.AnyAsync (r => r.FullName == name && r.Id != command.StudentModel.Id && !r.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Student"], name));
        }
    }
}
