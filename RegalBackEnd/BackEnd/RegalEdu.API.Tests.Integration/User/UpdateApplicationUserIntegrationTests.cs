using System.Net.Http.Json;
using FluentAssertions;
using RegalEdu.API.Tests.Integration.Common;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Exceptions;
using Xunit;
using RegalEdu.Api.Tests.Integration;

namespace RegalEdu.API.Tests.Integration.User
{
    public class UpdateApplicationUserIntegrationTests : BaseIntegrationTest
    {
        public UpdateApplicationUserIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact(DisplayName = "Should update application user successfully")]
        public async Task Should_Update_ApplicationUser_Successfully()
        {
            // 1️⃣ Add user
            var newUser = new ApplicationUserModel
            {
                UserName = $"integrationuser_{Guid.NewGuid()}",
                Email = $"integrationtest_{Guid.NewGuid()}@example.com",
                PhoneNumber = "0987654321",
                FullName = "Integration Test",
                UserCode = "TEST-ATID-123",
                Gender = true,
                Password = "Test123@123",
                PasswordConfirm = "Test123@123",
                IsDeleted = false,
                GenderText = "Male"
            };

            await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", newUser);

            // 2️⃣ Get user
            var getResponse = await Client.GetAsync("/api/RegalEduManagement/User/GetApplicationUsers");
            getResponse.EnsureSuccessStatusCode();

            var users = await getResponse.Content.ReadFromJsonAsync<List<ApplicationUserModel>>();
            users.Should().NotBeNullOrEmpty();

            var createdUser = users!.FirstOrDefault(u => u.Email == newUser.Email);
            createdUser.Should().NotBeNull();
            createdUser!.Id.Should().NotBe(Guid.Empty);

            // 3️⃣ Update user
            createdUser.FullName = "Updated Integration Test";
            createdUser.PhoneNumber = "0123456789";

            Console.WriteLine($"[Test] Updating user Id: {createdUser.Id}");

            var updateResponse = await Client.PutAsJsonAsync("/api/RegalEduManagement/User/UpdateApplicationUser", createdUser);
            updateResponse.EnsureSuccessStatusCode();

            var updateResult = await updateResponse.Content.ReadFromJsonAsync<Result>();
            updateResult.Should().NotBeNull();
            updateResult!.Succeeded.Should().BeTrue();
            updateResult.Errors.Should().BeNullOrEmpty();

            Console.WriteLine("[Test] Update success ✅");
        }

        [Fact(DisplayName = "Should throw validation exception when email duplicate")]
        public async Task Should_Throw_ValidationException_When_Email_Duplicate()
        {
            // 1️⃣ Add user A
            var userA = new ApplicationUserModel
            {
                UserName = $"userA_{Guid.NewGuid()}",
                Email = $"emailA_{Guid.NewGuid()}@example.com",
                PhoneNumber = "1111111111",
                FullName = "User A",
                UserCode = "TEST-A",
                Gender = true,
                Password = "Test123@123",
                PasswordConfirm = "Test123@123",
                IsDeleted = false,
                GenderText = "Male"
            };

            await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", userA);

            // 2️⃣ Add user B
            var userB = new ApplicationUserModel
            {
                UserName = $"userB_{Guid.NewGuid()}",
                Email = $"emailB_{Guid.NewGuid()}@example.com",
                PhoneNumber = "2222222222",
                FullName = "User B",
                UserCode = "TEST-B",
                Gender = true,
                Password = "Test123@123",
                PasswordConfirm = "Test123@123",
                IsDeleted = false,
                GenderText = "Male"
            };

            await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", userB);

            // 3️⃣ Get user B
            var getResponse = await Client.GetAsync("/api/RegalEduManagement/User/GetApplicationUsers");
            getResponse.EnsureSuccessStatusCode();

            var users = await getResponse.Content.ReadFromJsonAsync<List<ApplicationUserModel>>();
            users.Should().NotBeNullOrEmpty();

            var userBFromDb = users!.FirstOrDefault(u => u.Email == userB.Email);
            userBFromDb.Should().NotBeNull();
            userBFromDb!.Id.Should().NotBe(Guid.Empty);

            // 4️⃣ Update user B → duplicate Email with user A
            userBFromDb.Email = userA.Email;

            Console.WriteLine($"[Test] Trying to update UserB with duplicate email: {userBFromDb.Email}");

            // 🚀 EXPECT THROW
            var exception = await Xunit.Assert.ThrowsAsync<SimpleValidationException>(async () =>
            {
                var updateResponse = await Client.PutAsJsonAsync("/api/RegalEduManagement/User/UpdateApplicationUser", userBFromDb);
                updateResponse.EnsureSuccessStatusCode(); // This will throw if status is not 200, otherwise content will be SimpleValidationException
            });

            exception.Should().NotBeNull();
            exception.Message.Should().NotBeNullOrWhiteSpace();

            Console.WriteLine($"[Test] Caught expected SimpleValidationException: {exception.Message}");
        }
    }
}
