using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Validators
{
    public class BaseHolidayModelValidator : AbstractValidator<HolidayModel>
    {
        public BaseHolidayModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.Name)
                .NotEmpty ( ).WithMessage (localizer["HolidayNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("HolidayNameMaxLength", 200));

            RuleFor (x => x.Date)
                .NotEmpty ( ).WithMessage (localizer["HolidayDateRequired"]);

            RuleFor (x => x.Description)
                .MaximumLength (1000).WithMessage (localizer.Format ("HolidayDescriptionMaxLength", 1000));

            RuleFor (x => x.Frequency)
                .InclusiveBetween ((byte)0, (byte)1)
                .WithMessage (localizer["HolidayFrequencyInvalid"]);
        }
    }
}
