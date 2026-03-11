using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;
using System.Text.RegularExpressions;

public class BaseEventModelValidator : AbstractValidator<EventModel>
{
    public BaseEventModelValidator(ILocalizationService localizer)
    {
        // 1️ Kiểm tra EventCode
        RuleFor (x => x.EventCode)
            // Không được để trống
            .NotEmpty ( ).WithMessage (localizer["EventCodeRequired"])
            // Giới hạn tối đa 10 ký tự
            .MaximumLength (10).WithMessage (localizer.Format ("EventCodeMaxLength", 10))
            // Kiểm tra định dạng dựa trên Category
            .Must ((model, eventCode) =>
            {
                // Xác định AutoCodeType tương ứng với Category
                AutoCodeType autoCodeType = model.Category switch
                {
                    EventCategory.Event => AutoCodeType.Event_SK,
                    EventCategory.Report => AutoCodeType.Event_BC,
                    //EventCategory.News => AutoCodeType.Event_TT,
                    // EventCategory.Link => AutoCodeType.Event_LK,
                    _ => throw new ApplicationException (
                            localizer.Format (LocalizationKey.InvalidEventCategory, model.Category))
                };

                // Lấy cấu hình prefix và độ dài
                var config = AutoCodeConfig.Get (autoCodeType);

                // Mẫu regex: Prefix + số với chiều dài cố định
                // Ví dụ: SK0001, BC0001, TT0001, LK0001
                string pattern = $"^{config.Prefix}\\d{{{config.Length}}}$";

                return Regex.IsMatch (eventCode ?? string.Empty, pattern);
            })
            // Thông báo lỗi khi không khớp định dạng
            .WithMessage (model =>
            {
                AutoCodeType autoCodeType = model.Category switch
                {
                    EventCategory.Event => AutoCodeType.Event_SK,
                    EventCategory.Report => AutoCodeType.Event_BC,
                    // EventCategory.News => AutoCodeType.Event_TT,
                    //  EventCategory.Link => AutoCodeType.Event_LK,
                    _ => AutoCodeType.Event_SK
                };

                var config = AutoCodeConfig.Get (autoCodeType);
                return localizer.Format ("EventCodeInvalidFormat", config.Prefix, config.Length);
            });


        // 2️ Kiểm tra EventName
        RuleFor (x => x.EventName)
            .NotEmpty ( ).WithMessage (localizer["EventNameRequired"])   // Bắt buộc nhập
            .MaximumLength (200).WithMessage (localizer.Format ("EventNameMaxLength", 200)); // Giới hạn 200 ký tự


        // 3️ Kiểm tra Description
        RuleFor (x => x.Description)
            .MaximumLength (1000).WithMessage (localizer.Format ("EventDescriptionMaxLength", 1000)); // Tối đa 1000 ký tự


        // 4️⃣ Kiểm tra Category
        RuleFor (x => x.Category)
            .IsInEnum ( ).WithMessage (localizer["EventCategoryInvalid"]); // Phải thuộc enum EventCategory
    }
}
