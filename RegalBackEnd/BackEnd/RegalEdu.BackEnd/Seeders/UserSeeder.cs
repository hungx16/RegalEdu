// Ignore Spelling: Admin

using Microsoft.AspNetCore.Identity;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Api.Seeders
{
    public static class UserSeeder
    {
        private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("UserSeeder");
        public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@regaledu.vn";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Super Admin",
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    
                };
                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, AppRoles.Admin);
                    Logger.LogInformation("Admin user created successfully: {Email}", user.Email);
                }
                else
                {
                    Logger.LogError("Admin user creation failed: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                    throw new Exception("Admin user creation failed");
                }
            }
        }

        public static async Task SeedTestUsers(UserManager<ApplicationUser> userManager)
        {
            var users = new[] {
            new { Email = "counselor@regaledu.vn", Role = AppRoles.Counselor },
            new { Email = "teacher@regaledu.vn", Role = AppRoles.Teacher },
            new { Email = "viewer@regaledu.vn", Role = AppRoles.Viewer }
        };

            foreach (var item in users)
            {
                if (await userManager.FindByEmailAsync(item.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        FullName = item.Role + " User",
                        CreatedAt = DateTime.UtcNow,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, "Test@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, item.Role);
                        Logger.LogInformation("{Role} user created: {Email}", item.Role, item.Email);
                    }
                    else
                    {
                        Logger.LogError("Failed to create user {Email}: {Errors}", item.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
                        throw new Exception($"User creation failed: {item.Email}");
                    }
                }
            }
        }
    }
}
