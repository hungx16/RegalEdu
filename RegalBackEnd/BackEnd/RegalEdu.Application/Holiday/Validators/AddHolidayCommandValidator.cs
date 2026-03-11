using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Holiday.Commands;

namespace RegalEdu.Application.Holiday.Validators
{
    public class AddHolidayCommandValidator : AbstractValidator<AddHolidayCommand>
    {
        public AddHolidayCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.HolidayModel)
                .SetValidator (new BaseHolidayModelValidator (localizer));

            RuleFor (x => x.HolidayModel)
                .MustAsync (async (model, cancel) =>
                    !await db.Holidays.AnyAsync (h =>
                        h.Name == model.Name &&
                        h.Date == model.Date &&
                        (!h.WorkingTimeConfigurationId.HasValue || h.WorkingTimeConfigurationId == model.WorkingTimeConfigurationId) &&
                        !h.IsDeleted, cancel))
                .WithMessage (model => localizer.Format ("HolidayAlreadyExists", model.HolidayModel.Name, model.HolidayModel.Date.ToString ("yyyy-MM-dd")));
        }
    }
}
