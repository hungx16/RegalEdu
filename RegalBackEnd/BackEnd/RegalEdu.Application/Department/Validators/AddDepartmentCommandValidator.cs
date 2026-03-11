using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Department.Validators
{
    public class AddDepartmentCommandValidator : AbstractValidator<AddDepartmentCommand>
    {
        public AddDepartmentCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {

            RuleFor (x => x.DepartmentModel)
                .SetValidator (new BaseDepartmentModelValidator (localizer));

            //// Kiểm tra mã phòng ban là duy nhất
            //RuleFor (x => x.DepartmentModel.DepartmentCode)
            //    .MustAsync (async (code, cancellation) =>
            //    {
            //        return !await dbContext.Departments.AnyAsync (d => d.DepartmentCode == code && !d.IsDeleted, cancellation);
            //    })
            //    .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelCodeAlreadyExists, localizer[EntityName.Department], name));

            // Kiểm tra tên phòng ban là duy nhất
            RuleFor (x => x.DepartmentModel.DepartmentName)
                .MustAsync (async (name, cancellation) =>
                {
                    return !await dbContext.Departments.AnyAsync (d => d.DepartmentName == name && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, localizer[EntityName.Department], name));
        }
    }
}
