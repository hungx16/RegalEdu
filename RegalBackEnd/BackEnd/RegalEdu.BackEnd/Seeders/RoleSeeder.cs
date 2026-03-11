


// Ignore Spelling: Edu

using Microsoft.AspNetCore.Identity;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Persistence.Seeders
{
    public class RoleSeeder
    {
        private static readonly ILogger Logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole(); // Ensure this extension method is available
        }).CreateLogger("RoleSeeder");

        public static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] {
                    AppRoles.Admin,
                    AppRoles.Counselor,
                    AppRoles.Teacher,
                    AppRoles.Viewer
                };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    if (result.Succeeded)
                    {
                        Logger.LogInformation("Created role: {Role}", role);
                    }
                    else
                    {
                        Logger.LogError("Failed to create role {Role}: {Errors}", role, string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}
