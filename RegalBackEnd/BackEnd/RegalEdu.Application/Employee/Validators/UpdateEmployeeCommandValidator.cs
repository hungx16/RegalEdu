using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Employee.Commands;

namespace RegalEdu.Application.Employee.Validators
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.EmployeeModel)
                .SetValidator (new BaseEmployeeModelValidator (localizer));

            // Check ApplicationUserId không trùng với Employee khác (ngoại trừ bản ghi đang sửa)
            RuleFor (x => x.EmployeeModel.ApplicationUserId)
                .MustAsync (async (command, userId, cancellation) =>
                    !await dbContext.Employees.AnyAsync (e => e.ApplicationUserId == userId
                        && e.Id != command.EmployeeModel.Id
                        && !e.IsDeleted, cancellation))
                .WithMessage ((command, userId) => localizer.Format ("EmployeeUserAlreadyExists", userId));

            // Check Email không trùng (ngoại trừ user hiện tại)
            RuleFor (x => x.EmployeeModel.ApplicationUser.Email)
                .NotEmpty ( )
                .WithMessage (localizer["EmailRequired"])
                .MustAsync (async (command, email, cancellation) =>
                {
                    // Lấy ApplicationUserId hiện tại của employee đang update
                    var currentUserId = command.EmployeeModel.ApplicationUserId;
                    return !await dbContext.ApplicationUsers
                        .AnyAsync (u => u.Email == email && u.Id != currentUserId && !u.IsDeleted, cancellation);
                })
                .WithMessage ((command, email) => localizer.Format ("EmailAlreadyExists", email));

            // Check mã số thuế không trùng (ngoại trừ employee hiện tại)
            RuleFor (x => x.EmployeeModel.EmployeeTax)
                .MustAsync (async (command, tax, cancellation) =>
                    string.IsNullOrWhiteSpace (tax) ||
                    !await dbContext.Employees
                        .AnyAsync (e => e.EmployeeTax == tax && e.Id != command.EmployeeModel.Id && !e.IsDeleted, cancellation))
                .WithMessage ((command, tax) => localizer.Format ("EmployeeTaxAlreadyExists", tax));

            // Check số điện thoại không trùng (ngoại trừ user hiện tại)
            RuleFor (x => x.EmployeeModel.ApplicationUser.PhoneNumber)
                .NotEmpty ( )
                .WithMessage (localizer["PhoneNumberRequired"])
                .MustAsync (async (command, phone, cancellation) =>
                {
                    var currentUserId = command.EmployeeModel.ApplicationUserId;
                    return !await dbContext.ApplicationUsers
                        .AnyAsync (u => u.PhoneNumber == phone && u.Id != currentUserId && !u.IsDeleted, cancellation);
                })
                .WithMessage ((command, phone) => localizer.Format ("PhoneNumberAlreadyExists", phone));

        }
    }
}
