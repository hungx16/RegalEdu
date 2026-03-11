using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using Xunit;
using RegalEdu.API.Tests.Integration.Common;
using RegalEdu.Api.Tests.Integration;

namespace RegalEdu.API.Tests.Integration.User
{
    public class AddApplicationUserIntegrationTests : BaseIntegrationTest
    {
        public AddApplicationUserIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Should_Add_ApplicationUser_Successfully()
        {
            var newUser = CreateTestUser();

            Console.WriteLine("[Test] Sending POST to /api/RegalEduManagement/User/AddApplicationUser");

            var response = await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", newUser);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Test] Response Status: {response.StatusCode}");
            Console.WriteLine($"[Test] Response Body: {responseBody}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Result>();
            result.Should().NotBeNull();
            result!.Succeeded.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task Should_Forbidden_When_User_Without_Add_Privilege()
        {
            var newUser = CreateTestUser();

            var groupId = "00000000-0000-0000-0000-000000000001"; // Thay bằng GroupId thực tế (string)
            var groupName = "NoAddUserGroup";
            var permissionIds = new List<string> { "PERMISSION_CODE_1", "PERMISSION_CODE_2" };

            var token = await BaseTestAuthHelper.GetTokenForUserWithoutAddPrivilege(Client, groupId, groupName, permissionIds);

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Client.DefaultRequestHeaders.Remove("FormName");
            Client.DefaultRequestHeaders.Add("FormName", "ApplicationUser");


            var response = await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", newUser);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Should_Return_Unauthorized_When_No_Login()
        {
            var newUser = CreateTestUser();

            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Remove("FormName");
            Client.DefaultRequestHeaders.Add("FormName", "ApplicationUser");

            var response = await Client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", newUser);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #region Helper

        private ApplicationUserModel CreateTestUser()
        {
            return new ApplicationUserModel
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
        }

        #endregion
    }
}
