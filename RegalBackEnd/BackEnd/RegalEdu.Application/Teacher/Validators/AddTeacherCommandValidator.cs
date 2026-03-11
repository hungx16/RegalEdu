using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Teacher.Commands;

namespace RegalEdu.Application.Teacher.Validators
{
    public class AddTeacherCommandValidator : AbstractValidator<AddTeacherCommand>
    {
        public AddTeacherCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.TeacherModel)
                .SetValidator (new BaseTeacherModelValidator (localizer));

            //RuleFor (x => x.TeacherModel.TeacherName)
            //    .MustAsync (async (name, cancellation) =>
            //        !await dbContext.Teachers.AnyAsync (r => r.TeacherName == name && !r.IsDeleted, cancellation))
            //    .WithMessage ((cmd, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Teacher"], name));
        }
    }
}
