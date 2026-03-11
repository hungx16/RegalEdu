using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Event.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Event.Validators
{
    public class AddEventCommandValidator : AbstractValidator<AddEventCommand>
    {
        public AddEventCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1️ Áp dụng validator cơ bản (EventCode, EventName, Description, Category)
            RuleFor(x => x.EventModel)
                .SetValidator(new BaseEventModelValidator(localizer));

            // 2️ Kiểm tra EventCode phải duy nhất
            RuleFor(x => x.EventModel.EventCode)
                .MustAsync(async (code, cancellation) =>
                {
                    return !await dbContext.Events
                        .AnyAsync(e => e.EventCode == code && !e.IsDeleted, cancellation);
                })
                .WithMessage((command, code) =>
                    localizer.Format(LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.Event], code));

            // 3️ Kiểm tra EventName phải duy nhất
            RuleFor(x => x.EventModel.EventName)
                .MustAsync(async (name, cancellation) =>
                {
                    return !await dbContext.Events
                        .AnyAsync(e => e.EventName == name && !e.IsDeleted, cancellation);
                })
                .WithMessage((command, name) =>
                    localizer.Format(LocalizationKey.ModelNameAlreadyExists,
                        localizer[EntityName.Event], name));
        }
    }
}
