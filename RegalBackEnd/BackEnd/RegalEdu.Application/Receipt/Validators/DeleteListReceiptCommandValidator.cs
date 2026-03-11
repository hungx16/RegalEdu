using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Receipt.Validators
{
    public class DeleteListReceiptCommandValidator : AbstractValidator<RegalEdu.Application.Receipt.Commands.DeleteListReceiptCommand>
    {
        public DeleteListReceiptCommandValidator()
        {
            RuleFor(x => x.ListIds)
                .NotNull()
                .Must(x => x!.Any())
                .WithMessage("ListIds must contain at least one element.");
        }
    }
}
