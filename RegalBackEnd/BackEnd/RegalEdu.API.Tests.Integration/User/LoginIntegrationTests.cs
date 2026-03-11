using System.Net.Http.Json;
using FluentAssertions;
using RegalEdu.Api.Tests.Integration;
using RegalEdu.API.Tests.Integration.Common;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using Xunit;

namespace RegalEdu.API.Tests.Integration.User
{
    public class LoginIntegrationTests : BaseIntegrationTest
    {
        public LoginIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        //[Fact(Skip = "TODO: Implement login test")]
        [Fact]
        public async Task Should_Login_Successfully()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "ntvinh195@gmail.com",
                Password = "Test123@123",
                RememberMe = true
            };

            Console.WriteLine("[Test] Sending POST to /api/RegalEduManagement/User/login");

            var response = await Client.PostAsJsonAsync("/api/RegalEduManagement/User/login", loginRequest);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Test] Response Status: {response.StatusCode}");
            Console.WriteLine($"[Test] Response Body: {responseBody}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IdentityResult>();
            result.Should().NotBeNull();
        }
    }
}
