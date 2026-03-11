using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.LectureType.Commands;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.LectureType.Validators
{
    public class UpdateLectureTypeCommandValidator : AbstractValidator<UpdateLectureTypeCommand>
    {
        public UpdateLectureTypeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.LectureTypeModel)
                .SetValidator (new BaseLectureTypeModelValidator (localizer));

            RuleFor (x => x.LectureTypeModel.LectureName)
                .MustAsync (async (cmd, name, cancel) =>
                    !await db.LectureTypes.AnyAsync (c =>
                        c.LectureName == name &&
                        c.Id != cmd.LectureTypeModel.Id &&
                        !c.IsDeleted, cancel))
                .WithMessage ((cmd, name) => localizer.Format ("LectureNameAlreadyExists", name));
        }
    }
}
