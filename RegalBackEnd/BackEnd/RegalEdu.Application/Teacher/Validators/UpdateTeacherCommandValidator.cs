using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Teacher.Commands;

namespace RegalEdu.Application.Teacher.Validators
{
    public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
    {
        public UpdateTeacherCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.TeacherModel)
                .SetValidator (new BaseTeacherModelValidator (localizer));

            //RuleFor (x => x.TeacherModel.TeacherCode)
            //    .MustAsync (async (command, code, cancellation) =>
            //        !await dbContext.Teachers.AnyAsync (r => r.TeacherCode == code && r.Id != command.TeacherModel.Id && !r.IsDeleted, cancellation))
            //    .WithMessage ((command, code) => localizer.Format ("ModelCodeAlreadyExists", localizer["Teacher"], code));

            //RuleFor (x => x.TeacherModel.TeacherName)
            //    .MustAsync (async (command, name, cancellation) =>
            //        !await dbContext.Teachers.AnyAsync (r => r.TeacherName == name && r.Id != command.TeacherModel.Id && !r.IsDeleted, cancellation))
            //    .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Teacher"], name));
        }
    }
}
