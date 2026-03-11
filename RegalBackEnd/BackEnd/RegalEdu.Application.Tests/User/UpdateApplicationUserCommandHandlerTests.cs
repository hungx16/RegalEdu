using Moq;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Tests.TestHelpers;
using RegalEdu.Application.User.Commands;
using RegalEdu.Application.User.Validators;
using RegalEdu.Domain.Models;
using Xunit;


namespace RegalEdu.Application.Tests.User
{
    public class UpdateApplicationUserCommandValidatorTests : BaseValidatorTests<UpdateApplicationUserCommandValidator, UpdateApplicationUserCommand>
    {
        private readonly Mock<IIdentityService> _identityServiceMock;

        public UpdateApplicationUserCommandValidatorTests( )
            : base ( )
        {
            _identityServiceMock = new Mock<IIdentityService> ( );

            Validator = new UpdateApplicationUserCommandValidator (new FakeLocalizationService ( ), _identityServiceMock.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Email_Duplicate( )
        {
            #region Arrange

            var command = new UpdateApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Id = Guid.NewGuid ( ),
                    Email = "duplicate@example.com"
                }
            };

            _identityServiceMock.Setup (x => x.IsEmailExistsForOtherUserAsync (command.ApplicationUserModel.Id!.Value, It.IsAny<string> ( )))
                .ReturnsAsync (true);

            #endregion

            #region Act + Assert

            await AssertHasValidationErrorAsync (command, x => x.ApplicationUserModel.Email, "duplicate@example.com");

            #endregion
        }

        [Fact]
        public async Task Should_Have_Error_When_UserName_Duplicate( )
        {
            #region Arrange

            var command = new UpdateApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Id = Guid.NewGuid ( ),
                    UserName = "duplicateuser"
                }
            };

            _identityServiceMock.Setup (x => x.IsUserNameExistsForOtherUserAsync (command.ApplicationUserModel.Id!.Value, It.IsAny<string> ( )))
                .ReturnsAsync (true);

            #endregion

            #region Act + Assert

            await AssertHasValidationErrorAsync (command, x => x.ApplicationUserModel.UserName, "duplicateuser");

            #endregion
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Valid( )
        {
            #region Arrange

            var command = new UpdateApplicationUserCommand
            {
                ApplicationUserModel = new ApplicationUserModel
                {
                    Id = Guid.NewGuid ( ),
                    UserName = "validuser",
                    Email = "valid@example.com"
                }
            };

            _identityServiceMock.Setup (x => x.IsUserNameExistsForOtherUserAsync (command.ApplicationUserModel.Id!.Value, It.IsAny<string> ( )))
                .ReturnsAsync (false);

            _identityServiceMock.Setup (x => x.IsEmailExistsForOtherUserAsync (command.ApplicationUserModel.Id!.Value, It.IsAny<string> ( )))
                .ReturnsAsync (false);

            #endregion

            #region Act + Assert

            await AssertNoValidationErrorsAsync (command);

            #endregion
        }
    }
}
