using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Degree.Commands;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.Degree.Validators
{
    public class UpdateDegreeCommandValidator : AbstractValidator<UpdateDegreeCommand>
    {
        public UpdateDegreeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.DegreeModel)
                .SetValidator (new BaseDegreeModelValidator (localizer));

            RuleFor (x => x.DegreeModel.DegreeName)
                .MustAsync (async (cmd, name, cancel) =>
                    !await db.Degrees.AnyAsync (d =>
                        d.DegreeName == name &&
                        d.Id != cmd.DegreeModel.Id &&
                        !d.IsDeleted, cancel))
                .WithMessage ((cmd, name) => localizer.Format ("DegreeNameAlreadyExists", name));
        }
    }
}
