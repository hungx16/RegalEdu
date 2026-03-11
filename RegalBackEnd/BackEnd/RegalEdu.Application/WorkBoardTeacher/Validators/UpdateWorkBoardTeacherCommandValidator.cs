using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.WorkBoardTeacher.Commands;

namespace RegalEdu.Application.WorkBoardTeacher.Validators
{
    public class UpdateWorkBoardTeacherCommandValidator : AbstractValidator<UpdateWorkBoardTeacherCommand>
    {
        public UpdateWorkBoardTeacherCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor(x => x.WorkBoardTeacherModel.TeacherId)
                .NotEmpty().WithMessage(localizer["TeacherRequired"]);

            RuleFor(x => x.WorkBoardTeacherModel.Date)
                .NotEmpty().WithMessage(localizer["DateRequired"])
                .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage(localizer["DateCannotBeFuture"]);

            RuleFor(x => x.WorkBoardTeacherModel.Status)
                .InclusiveBetween(1, 3).WithMessage(localizer["StatusInvalid"]);

            RuleFor(x => x.WorkBoardTeacherModel.Location)
                .MaximumLength(1000).WithMessage(localizer.Format("LocationMaxLength", 1000));

            RuleFor(x => x.WorkBoardTeacherModel.Note)
                .MaximumLength(2000).WithMessage(localizer.Format("NoteMaxLength", 2000));

            // Kiểm tra trùng lặp work board cho cùng giáo viên trong cùng ngày (trừ bản ghi hiện tại)
            RuleFor(x => x.WorkBoardTeacherModel)
                .MustAsync(async (command, model, cancellation) =>
                {
                    var existing = await dbContext.WorkBoardTeachers.AnyAsync(wbt =>
                        wbt.TeacherId == model.TeacherId &&
                        wbt.Date == model.Date.Date &&
                        wbt.Id != command.WorkBoardTeacherModel.Id &&
                        !wbt.IsDeleted, cancellation);
                    return !existing;
                })
                .WithMessage((command, model) => localizer["WorkBoardForTeacherDateAlreadyExists"]);

            // Kiểm tra nếu có checkin và checkout thì checkout phải sau checkin
            RuleFor(x => x.WorkBoardTeacherModel)
                .Must(model => !model.CheckinTime.HasValue || !model.CheckoutTime.HasValue || model.CheckoutTime > model.CheckinTime)
                .WithMessage(localizer["CheckoutTimeMustAfterCheckinTime"]);
        }
    }
}