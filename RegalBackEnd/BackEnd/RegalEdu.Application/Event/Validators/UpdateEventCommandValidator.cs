using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Event.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Event.Validators
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // 1️ Áp dụng validator cơ bản cho EventModel
            RuleFor(x => x.EventModel)
                .SetValidator(new BaseEventModelValidator(localizer));

            // 2️ Kiểm tra EventCode duy nhất (ngoại trừ chính bản ghi đang sửa)
            RuleFor(x => x.EventModel.EventCode)
                .MustAsync(async (command, eventCode, cancellation) =>
                {
                    return !await dbContext.Events.AnyAsync(
                        e => e.EventCode == eventCode
                          && e.Id != command.EventModel.Id
                          && !e.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, eventCode) =>
                    localizer.Format(
                        LocalizationKey.ModelCodeAlreadyExists,
                        localizer[EntityName.Event],
                        eventCode));

            // 3️ Kiểm tra EventName duy nhất (ngoại trừ chính bản ghi đang sửa)
            RuleFor(x => x.EventModel.EventName)
                .MustAsync(async (command, eventName, cancellation) =>
                {
                    return !await dbContext.Events.AnyAsync(
                        e => e.EventName == eventName
                          && e.Id != command.EventModel.Id
                          && !e.IsDeleted,
                        cancellation);
                })
                .WithMessage((command, eventName) =>
                    localizer.Format(
                        LocalizationKey.ModelNameAlreadyExists,
                        localizer[EntityName.Event],
                        eventName));
        }
    }
}
