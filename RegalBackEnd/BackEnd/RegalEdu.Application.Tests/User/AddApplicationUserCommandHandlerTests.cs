using FluentValidation.TestHelper;
using Moq;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.User.Commands;
using RegalEdu.Application.User.Validators;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Tests.TestHelpers;
using Xunit;
using System;
using System.Threading.Tasks;

namespace RegalEdu.Application.Tests.User
{
    public class AddApplicationUserCommandValidatorTests : BaseValidatorTests<AddApplicationUserCommandValidator, AddApplicationUserCommand>
    {
        private readonly Mock<IIdentityService> _identityServiceMock;

        public AddApplicationUserCommandValidatorTests()
            : base()
        {
            _identityServiceMock = new Mock<IIdentityService>();

            Validator = new AddApplicationUserCommandValidator(new FakeLocalizationService(), _identityServiceMock.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Duplicate()
        {
            var command = new AddApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Email = "duplicate@example.com",
                    UserName = "testuser",
                    Password = "Password@123"
                }
            };

            _identityServiceMock.Setup(x => x.IsEmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            await AssertHasValidationErrorAsync(command, x => x.ApplicationUserModel.Email, "duplicate@example.com");
        }

        [Fact]
        public async Task Should_Have_Error_When_UserName_Duplicate()
        {
            var command = new AddApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Email = "test@example.com",
                    UserName = "duplicateuser",
                    Password = "Password@123"
                }
            };

            _identityServiceMock.Setup(x => x.IsUserNameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            await AssertHasValidationErrorAsync(command, x => x.ApplicationUserModel.UserName, "duplicateuser");
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Valid()
        {
            var command = new AddApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Email = "valid@example.com",
                    UserName = "validuser",
                    Password = "Password@123"
                }
            };

            _identityServiceMock.Setup(x => x.IsUserNameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _identityServiceMock.Setup(x => x.IsEmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            await AssertNoValidationErrorsAsync(command);
        }
    }
}
