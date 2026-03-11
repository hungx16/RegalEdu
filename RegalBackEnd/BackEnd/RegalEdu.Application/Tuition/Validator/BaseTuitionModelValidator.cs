using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Tuition.Validators
{
    public class BaseTuitionModelValidator : AbstractValidator<TuitionModel>
    {
        public BaseTuitionModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.TuitionName)
                .NotEmpty ( ).WithMessage (localizer["TuitionNameRequired"])
                .MaximumLength (255).WithMessage (localizer.Format ("TuitionNameMaxLength", 255));

            RuleFor (x => x.CourseId)
                .NotEmpty ( ).WithMessage (localizer["CourseRequired"]);

            RuleFor (x => x.ClassTypeId)
                .NotEmpty ( ).WithMessage (localizer["ClassTypeRequired"]);

            RuleFor (x => x.DurationHours)
                .GreaterThan (0).WithMessage (localizer["DurationHoursPositive"])
                .LessThanOrEqualTo (1000).WithMessage (localizer.Format ("DurationHoursMax", 1000));

            RuleFor (x => x.MinHours)
                .GreaterThan (0).WithMessage (localizer["MinHoursPositive"])
                .LessThanOrEqualTo (x => x.DurationHours)
                .WithMessage (localizer["MinHoursLessThanDuration"]);

            RuleFor (x => x.TotalMonths)
                .LessThanOrEqualTo (100).WithMessage (localizer.Format ("TotalMonthsMax", 100));

            RuleFor (x => x.TuitionFee)
                .GreaterThanOrEqualTo (0).WithMessage (localizer["TuitionFeeNonNegative"]);
        }
    }
}
