using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.WorkingTime.Commands;

namespace RegalEdu.Application.WorkingTime.Validators
{
    public class UpdateWorkingTimeCommandValidator : AbstractValidator<UpdateWorkingTimeCommand>
    {
        public UpdateWorkingTimeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.WorkingTimeModel)
                .SetValidator (new BaseWorkingTimeModelValidator (localizer));
            RuleFor (x => x.WorkingTimeModel)
                .MustAsync (async (model, cancel) =>
                    !await db.WorkingTimes.AnyAsync (w =>
                        w.Name == model.Name &&
                        w.DayOfWeek == model.DayOfWeek &&
                        w.Id != model.Id &&
                        !w.IsDeleted && w.WorkingTimeConfigurationId == model.WorkingTimeConfigurationId, cancel))
                .WithMessage (model =>
                {
                    var dayOfWeekName = localizer[$"DayOfWeek_{model.WorkingTimeModel.DayOfWeek}"];
                    return localizer.Format ("ModelNameAlreadyExistsWithDay", localizer["WorkingTime"], model.WorkingTimeModel.Name, dayOfWeekName);
                });
        }
    }
}
