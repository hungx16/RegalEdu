using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Infrastructure.Identity;
using RegalEdu.Infrastructure.Repositories;
using RegalEdu.Infrastructure.Services;
using RegalEducation.Persistence;
using RegalEdu.Application.Notifications.Interfaces;

namespace RegalEdu.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped (typeof (IRegalEducationRepository<>), typeof (RegalEducationRepository<>));
            services.AddScoped<INotificationRepository, NotificationRepository> ( );
            services.AddTransient<IIdentityService, IdentityService> ( );
            services.AddScoped<IRegalEducationDbContext> (provider => provider.GetRequiredService<RegalEducationDbContext> ( ));
            services.AddHostedService<JwtRefreshTokenCache> ( );
            services.AddSingleton<IJwtAuthManager, JwtAuthManager> ( );
            return services;
        }
    }
}
