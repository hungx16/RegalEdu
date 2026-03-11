using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegalEdu.Api.Tests.Integration;
using Xunit;

namespace RegalEdu.API.Tests.Integration.Common
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected readonly IConfiguration Configuration;

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();
            Configuration = factory.Services.GetRequiredService<IConfiguration>();

            // Generate token
            var token = AuthHelper.GenerateTestJwtToken(Configuration);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Log for debug
            Console.WriteLine($"[BaseIntegrationTest] JWT Token: {token}");
        }
    }
}
