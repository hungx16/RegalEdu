using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Validators
{
    public class BaseEmployeeModelValidator : AbstractValidator<EmployeeModel>
    {
        public BaseEmployeeModelValidator(ILocalizationService localizer)
        {
            //RuleFor (x => x.ApplicationUserId)
            //    .NotEmpty ( ).WithMessage (localizer["ApplicationUserIdRequired"]);
            string start = AutoCodeConfig.Get (AutoCodeType.Employee).Prefix;
            int length = AutoCodeConfig.Get (AutoCodeType.Employee).Length;

            RuleFor (x => x.ApplicationUser.UserCode)
                .NotEmpty ( ).WithMessage (localizer["EmployeeCodeRequired"])
                .MaximumLength (10).WithMessage (localizer.Format ("EmployeeCodeMaxLength", 10))
                .Matches ($"^{start}\\d{{{length}}}$")
                .WithMessage (localizer.Format ("EmployeeCodeInvalidFormat", start, length));
            RuleFor (x => x.CompanyId)
                .NotEmpty ( ).WithMessage (localizer["CompanyIdRequired"]);
            RuleFor (x => x.PositionId)
                .NotEmpty ( ).WithMessage (localizer["PositionIdRequired"]);
            RuleFor (x => x.DepartmentId)
                .NotEmpty ( ).WithMessage (localizer["DepartmentIdRequired"]);
            RuleFor (x => x.EmployeeTax)
                .MaximumLength (100).WithMessage (localizer.Format ("EmployeeTaxMaxLength", 100));
            // Có thể bổ sung các rule khác theo yêu cầu nghiệp vụ
        }
    }
}
