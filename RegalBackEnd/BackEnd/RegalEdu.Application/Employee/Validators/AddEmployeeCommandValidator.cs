using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Employee.Commands;

namespace RegalEdu.Application.Employee.Validators
{
    public class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
    {
        public AddEmployeeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            // Rule validate các field cơ bản
            RuleFor (x => x.EmployeeModel)
                .SetValidator (new BaseEmployeeModelValidator (localizer));

            // Rule kiểm tra ApplicationUserId không trùng (Employee chưa bị xóa)
            RuleFor (x => x.EmployeeModel.ApplicationUserId)
                .MustAsync (async (userId, cancellation) => string.IsNullOrWhiteSpace (userId.ToString ( )) ||
                    !await dbContext.Employees.AnyAsync (e => e.ApplicationUserId == userId && !e.IsDeleted, cancellation))
                .WithMessage ((cmd, userId) => localizer.Format ("EmployeeUserAlreadyExists", userId));

            // Rule kiểm tra Email không trùng trong ApplicationUser (vẫn còn active)
            RuleFor (x => x.EmployeeModel!.ApplicationUser!.Email)
                .NotEmpty ( )
                .WithMessage (localizer["EmailRequired"])
                .MustAsync (async (email, cancellation) =>
                    !await dbContext.ApplicationUsers.AnyAsync (u => u.Email == email && !(u.IsDeleted == true), cancellation))
                .WithMessage ((cmd, email) => localizer.Format ("EmailAlreadyExists", email));

            // Rule kiểm tra mã số thuế (EmployeeTax) không trùng (nếu có nhập)
            RuleFor (x => x.EmployeeModel.EmployeeTax)
                .MustAsync (async (tax, cancellation) =>
                    string.IsNullOrWhiteSpace (tax) ||
                    !await dbContext.Employees.AnyAsync (e => e.EmployeeTax == tax && !e.IsDeleted, cancellation))
                .WithMessage ((cmd, tax) => localizer.Format ("EmployeeTaxAlreadyExists", tax));


            // Check số điện thoại không trùng (ngoại trừ user hiện tại)
            RuleFor (x => x.EmployeeModel.ApplicationUser.PhoneNumber)
                .NotEmpty ( )
                .WithMessage (localizer["PhoneNumberRequired"])
                .MustAsync (async (command, phone, cancellation) =>
                {
                    var currentUserId = command.EmployeeModel.ApplicationUserId;
                    return !await dbContext.ApplicationUsers
                        .AnyAsync (u => u.PhoneNumber == phone && !u.IsDeleted, cancellation);
                })
                .WithMessage ((command, phone) => localizer.Format ("PhoneNumberAlreadyExists", phone));
        }
    }
}
