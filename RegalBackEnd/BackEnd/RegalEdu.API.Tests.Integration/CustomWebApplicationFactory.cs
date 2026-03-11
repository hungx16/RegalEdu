using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace RegalEdu.Api.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<ProgramForTesting>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set Environment là "Testing"
            builder.UseEnvironment("Testing");

            // Load cấu hình Testing
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables();
            });

            // KHÔNG cần override ConfigureServices nữa!
        }
    }
}
