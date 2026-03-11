using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.PayrollTeacher.Commands;

namespace RegalEdu.Application.PayrollTeacher.Validators
{
    public class UpdatePayrollTeacherCommandValidator : AbstractValidator<UpdatePayrollTeacherCommand>
    {
        public UpdatePayrollTeacherCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.PayrollTeacherModel.TeacherId)
                .NotEmpty().WithMessage(localizer["TeacherRequired"]);

            RuleFor(x => x.PayrollTeacherModel.SalaryMonth)
                .NotEmpty().WithMessage(localizer["SalaryMonthRequired"])
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(localizer["SalaryMonthCannotBeFuture"]);

            RuleFor(x => x.PayrollTeacherModel.StandardWorkDay)
                .GreaterThanOrEqualTo(0).WithMessage(localizer["StandardWorkDayMustBePositive"])
                .LessThanOrEqualTo(31).WithMessage(localizer["StandardWorkDayMaxExceeded"]);

            RuleFor(x => x.PayrollTeacherModel.ActualWorkDay)
                .GreaterThanOrEqualTo(0).WithMessage(localizer["ActualWorkDayMustBePositive"])
                .LessThanOrEqualTo(31).WithMessage(localizer["ActualWorkDayMaxExceeded"]);

            RuleFor(x => x.PayrollTeacherModel.SalaryAmount)
                .GreaterThanOrEqualTo(0).WithMessage(localizer["SalaryAmountMustBePositive"]);

            // Kiểm tra trùng lặp payroll cho cùng giáo viên trong cùng tháng (trừ bản ghi hiện tại)
            RuleFor(x => x.PayrollTeacherModel)
                .MustAsync(async (command, model, cancellation) =>
                {
                    var existing = await dbContext.PayrollTeachers.AnyAsync(pt =>
                        pt.TeacherId == model.TeacherId &&
                        pt.SalaryMonth.Year == model.SalaryMonth.Year &&
                        pt.SalaryMonth.Month == model.SalaryMonth.Month &&
                        pt.Id != command.PayrollTeacherModel.Id &&
                        !pt.IsDeleted, cancellation);
                    return !existing;
                })
                .WithMessage((command, model) => localizer["PayrollForTeacherMonthAlreadyExists"]);
        }
    }
}