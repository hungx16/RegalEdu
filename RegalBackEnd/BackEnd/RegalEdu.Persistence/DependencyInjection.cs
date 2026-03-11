using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegalEdu.Domain.Entities;
using RegalEducation.Persistence;


namespace RegalEdu.Persistence
{
    public static class DependencyInjection
    {
        //    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        //    {
        //        services.AddDbContext<RegalEducationDbContext>(options =>
        //            options.UseSqlServer(configuration.GetConnectionString("RegalEduDatabase"),
        //            b => b.MigrationsAssembly(typeof(RegalEducationDbContext).Assembly.FullName)));

        //        services.AddScoped<RegalEducationDbContext>(provider =>
        //            provider.GetRequiredService<RegalEducationDbContext>()); // Use GetRequiredService to ensure non-null return  

        //        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        //        {
        //            options.Password.RequireDigit = false;
        //            options.Password.RequireLowercase = false;
        //            options.Password.RequireNonAlphanumeric = false;
        //            options.Password.RequireUppercase = false;
        //            options.Password.RequiredLength = 6;
        //            options.Password.RequiredUniqueChars = 0;

        //            options.SignIn.RequireConfirmedAccount = false;

        //            options.Lockout.AllowedForNewUsers = false;
        //            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(36500);
        //            options.Lockout.MaxFailedAccessAttempts = 10;
        //        })
        //.AddEntityFrameworkStores<RegalEducationDbContext>();

        //        return services;
        //    }
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext
            services.AddDbContext<RegalEducationDbContext> (options =>
                options.UseSqlServer (configuration.GetConnectionString ("RegalEduDatabase"),

                b => b.MigrationsAssembly (typeof (RegalEducationDbContext).Assembly.FullName)));

            services.AddDbContext<RegalEducationDbContext> (options =>
            {
                options.UseSqlServer (configuration.GetConnectionString ("RegalEduDatabase"),
                    o => o.UseQuerySplittingBehavior (QuerySplittingBehavior.SplitQuery)); // 👈
            });
            // Đăng ký Identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>> (options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.SignIn.RequireConfirmedAccount = false;

                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays (36500);
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
            .AddEntityFrameworkStores<RegalEducationDbContext> ( )
            .AddDefaultTokenProviders ( );

            return services;
        }

    }

    public static class TestPersistenceServiceRegistration
    {
        public static IServiceCollection AddTestPersistence(this IServiceCollection services)
        {
            // Dùng InMemory DB cho Integration Test
            services.AddDbContext<RegalEducationDbContext> (options =>
                options.UseInMemoryDatabase ("IntegrationTestsDb"));

            // Đăng ký Identity giống AddPersistence
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>> (options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.SignIn.RequireConfirmedAccount = false;

                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays (36500);
                options.Lockout.MaxFailedAccessAttempts = 10;
            })
            .AddEntityFrameworkStores<RegalEducationDbContext> ( )
            .AddDefaultTokenProviders ( );

            return services;
        }
    }
}