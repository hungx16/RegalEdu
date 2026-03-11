using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Position.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Position.Validators
{
    public class AddPositionCommandValidator : AbstractValidator<AddPositionCommand>
    {
        public AddPositionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.PositionModel)
                .SetValidator (new BasePositionModelValidator (localizer));

            // Kiểm tra mã chức vụ là duy nhất
            RuleFor (x => x.PositionModel.PositionCode)
                .MustAsync (async (code, cancellation) =>
                {
                    return !await dbContext.Positions.AnyAsync (d => d.PositionCode == code && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelCodeAlreadyExists, EntityName.Position, name));


            RuleFor (x => x.PositionModel.PositionName)
                .MustAsync (async (name, cancellation) =>
                    !await dbContext.Positions.AnyAsync (d => d.PositionName == name && !d.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, EntityName.Position, name));
        }
    }
}
