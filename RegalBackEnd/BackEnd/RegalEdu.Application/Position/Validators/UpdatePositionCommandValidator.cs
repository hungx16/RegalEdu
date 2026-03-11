using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Position.Commands;

namespace RegalEdu.Application.Position.Validators
{
    public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
    {
        public UpdatePositionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.PositionModel)
                .SetValidator (new BasePositionModelValidator (localizer));

            RuleFor (x => x.PositionModel.PositionCode)
                .MustAsync (async (command, code, cancellation) =>
                    !await dbContext.Positions.AnyAsync (d => d.PositionCode == code && d.Id != command.PositionModel.Id && !d.IsDeleted, cancellation))
                .WithMessage ((command, code) => localizer.Format ("ModelCodeAlreadyExists", localizer["Position"], code));

            RuleFor (x => x.PositionModel.PositionName)
                .MustAsync (async (command, name, cancellation) =>
                    !await dbContext.Positions.AnyAsync (d => d.PositionName == name && d.Id != command.PositionModel.Id && !d.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Position"], name));
        }
    }
}
