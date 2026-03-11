using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Department.Validators
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.DepartmentModel).SetValidator (new BaseDepartmentModelValidator (localizer));

            // Kiểm tra mã phòng ban là duy nhất (trừ chính bản ghi đang sửa)
            RuleFor (x => x.DepartmentModel.DepartmentCode)
                .MustAsync (async (command, departmentCode, cancellation) =>
                {
                    return !await dbContext.Departments.AnyAsync (
                        d => d.DepartmentCode == departmentCode
                          && d.Id != command.DepartmentModel.Id
                          && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, departmentCode) => localizer.Format (LocalizationKey.ModelCodeAlreadyExists, localizer[EntityName.Department], departmentCode));

            // Kiểm tra tên phòng ban là duy nhất (trừ chính bản ghi đang sửa)
            RuleFor (x => x.DepartmentModel.DepartmentName)
                .MustAsync (async (command, departmentName, cancellation) =>
                {
                    return !await dbContext.Departments.AnyAsync (
                        d => d.DepartmentName == departmentName
                          && d.Id != command.DepartmentModel.Id
                          && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, departmentName) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, localizer[EntityName.Department], departmentName));
        }
    }
}
