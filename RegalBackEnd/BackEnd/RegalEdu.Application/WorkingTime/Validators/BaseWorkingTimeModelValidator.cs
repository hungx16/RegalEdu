using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Validators
{
    public class BaseWorkingTimeModelValidator : AbstractValidator<WorkingTimeModel>
    {
        public BaseWorkingTimeModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.Name)
                .NotEmpty ( ).WithMessage (localizer["WorkingTimeNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("WorkingTimeNameMaxLength", 200));

            RuleFor (x => x.StartTime)
                .NotNull ( ).WithMessage (localizer["WorkingTimeStartTimeRequired"]);

            RuleFor (x => x.EndTime)
                .NotNull ( ).WithMessage (localizer["WorkingTimeEndTimeRequired"]);

            RuleFor (x => x.DayOfWeek)
                .InclusiveBetween ((byte)0, (byte)6)
                .WithMessage (localizer["WorkingTimeDayOfWeekInvalid"]);
            RuleFor (x => x)
              .Must (x => x.StartTime < x.EndTime)
              .WithMessage (localizer["WorkingTimeStartTimeMustBeLessThanEndTime"]);
        }
    }
}
