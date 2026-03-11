using System.Net;
using System.Text;
using FluentAssertions;
using RegalEdu.Api.Tests.Integration;
using RegalEdu.API.Tests.Integration.Common;
using Xunit;

namespace RegalEdu.API.Tests.Integration.Middleware
{
    public class CheckPrivilegeMiddlewareTests : BaseIntegrationTest
    {
        public CheckPrivilegeMiddlewareTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Post_Should_Return_Forbidden_When_User_Lacks_Privilege()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/RegalEduManagement/User/AddApplicationUser")
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
            request.Headers.Add("formname", "FakeForm");

            var response = await Client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Should_Return_Forbidden_When_User_Lacks_Privilege()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/RegalEduManagement/User/DeleteListUser?arrUserId=1");
            request.Headers.Add("formname", "FakeForm");

            var response = await Client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
