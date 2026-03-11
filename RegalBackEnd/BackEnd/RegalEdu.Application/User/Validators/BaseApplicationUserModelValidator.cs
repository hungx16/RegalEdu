
using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.User.Commands;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.User.Validators
{
    public class BaseApplicationUserModelValidator : AbstractValidator<ApplicationUserModel>
    {
        public BaseApplicationUserModelValidator(ILocalizationService localizer, IIdentityService identityService)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer["UserNameRequired"])
                .MaximumLength(100).WithMessage(localizer.Format("UserNameMaxLength", 100));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer["EmailRequired"])
                .EmailAddress().WithMessage(localizer["EmailInvalid"]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer["PasswordCannotBeEmpty"])
                .MinimumLength(6).WithMessage(localizer.Format("PasswordMinLength", 6))
                .When((model, context) =>
                {
                    if (context.RootContextData.TryGetValue("SkipPassword", out var skip)
                        && skip is bool skipPassword && skipPassword)
                    {
                        return false; // Skip Rule
                    }
                    return true; // Apply Rule
                });

            RuleFor(x => x.UserCode)
                .MaximumLength(50).WithMessage(localizer.Format("EmployeeATIDMaxLength", 50));
        }
    }
    public class AddApplicationUserCommandValidator : AbstractValidator<AddApplicationUserCommand>
    {
        public AddApplicationUserCommandValidator(ILocalizationService localizer, IIdentityService identityService)
        {
            RuleFor(x => x.ApplicationUserModel)
                .SetValidator(new BaseApplicationUserModelValidator(localizer, identityService));

            RuleFor(x => x.ApplicationUserModel.Email)
                .MustAsync(async (email, cancellation) =>
                {
                    return !await identityService.IsEmailExistsAsync(email ?? string.Empty);
                })
                .WithMessage((command, email) => localizer.Format("EmailAlreadyExists", email));


            RuleFor(x => x.ApplicationUserModel.UserName)
                           .MustAsync(async (userName, cancellation) =>
                           {
                               return !await identityService.IsUserNameExistsAsync(userName ?? string.Empty);
                           })
                           .WithMessage((command, userName) => localizer.Format("UserNameAlreadyExists", userName));
        }
    }
    public class UpdateApplicationUserCommandValidator : AbstractValidator<UpdateApplicationUserCommand>
    {
        public UpdateApplicationUserCommandValidator(ILocalizationService localizer, IIdentityService identityService)
        {
            RuleFor(x => x.ApplicationUserModel)
                .Custom((_, context) =>
                {
                    context.RootContextData["SkipPassword"] = true;
                });
            RuleFor(x => x.ApplicationUserModel)
                .SetValidator(new BaseApplicationUserModelValidator(localizer, identityService));


            RuleFor(command => command.ApplicationUserModel.UserName)
                .MustAsync(async (command, userName, cancellation) =>
                {
                    return !await identityService.IsUserNameExistsForOtherUserAsync(
                        command.ApplicationUserModel.Id ?? Guid.Empty,
                        userName ?? string.Empty);
                })
                .WithMessage(localizer.Format("UserNameAlreadyExists", "{PropertyValue}"));

            RuleFor(command => command.ApplicationUserModel.Email)
                .MustAsync(async (command, email, cancellation) =>
                {
                    return !await identityService.IsEmailExistsForOtherUserAsync(
                        command.ApplicationUserModel.Id ?? Guid.Empty,
                        email ?? string.Empty);
                })
                .WithMessage(localizer.Format("EmailAlreadyExists", "{PropertyValue}"))
                .OverridePropertyName("ApplicationUserModel.Email");
        }
    }
}
