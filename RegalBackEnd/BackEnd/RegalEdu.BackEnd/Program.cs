using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using RegalEdu.Api.Hubs;
using RegalEdu.Api.Middleware;
using RegalEdu.Api.Seeders;
using RegalEdu.Application;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Behaviors;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Services;
using RegalEdu.Application.Common.Settings;
using RegalEdu.BackEnd.Middleware;
using RegalEdu.Domain.Entities;
using RegalEdu.Infrastructure;
using RegalEdu.Infrastructure.Identity;
using RegalEdu.Application.Notifications.Interfaces;
using RegalEdu.Infrastructure.Services;
using RegalEdu.Persistence;
using RegalEdu.Persistence.Seeders;
using RegalEducation.Persistence;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System.Text;
using System.Text.Json.Serialization;

#region Serilog Configuration
Log.Logger = new LoggerConfiguration ( )
    .MinimumLevel.Debug ( )
    .MinimumLevel.Override ("Microsoft", LogEventLevel.Warning) // Giảm noise Microsoft log
    .Enrich.FromLogContext ( )
    .Filter.ByExcluding (Matching.FromSource ("Hangfire"))

    // INFO only
    .WriteTo.Logger (lc => lc
        .Filter.ByIncludingOnly (e => e.Level == LogEventLevel.Information || e.Level == LogEventLevel.Warning)
        .WriteTo.File ("Logs/info-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate:
            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))

    // ERROR only
    .WriteTo.Logger (lc => lc
        .Filter.ByIncludingOnly (e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
        .WriteTo.File ("Logs/error-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 60,
            outputTemplate:
            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))

    // Console (tất cả)
    .WriteTo.Console (outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")

    .CreateLogger ( );
#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder (new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory ( ),
    EnvironmentName = Environment.GetEnvironmentVariable ("ASPNETCORE_ENVIRONMENT") ?? "Production"
});


builder.Configuration
    .SetBasePath (Directory.GetCurrentDirectory ( ))
    .AddJsonFile ("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile ($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables ( );

builder.Host.UseSerilog ( );
#endregion
//Hải 15.12.2025
builder.Services.AddTransient<IDateCalculator, DateCalculator>();


// Kích hoạt tự động kiểm tra (auto validation)
//builder.Services.AddFluentValidationAutoValidation();

// Đăng ký MediatR để command chạy được
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCategoryCommand).Assembly));


// Replace the problematic line with the correct method call.  
builder.Services.AddValidatorsFromAssemblyContaining<ILocalizationService> ( );

builder.Services.AddTransient (typeof (IPipelineBehavior<,>), typeof (ValidationBehaviour<,>));
#region Hangfire Configuration
builder.Services.AddHangfire (cfg => cfg
    .SetDataCompatibilityLevel (CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer ( )
    .UseRecommendedSerializerSettings ( )
    .UseSqlServerStorage (
        builder.Configuration.GetConnectionString ("HangfireConnection")
    )
);
builder.Services.AddHangfireServer (options =>
{
    options.WorkerCount = Environment.ProcessorCount; // ví dụ
    options.Queues = new[] { "default", "emails", "maintenance" };
});
GlobalJobFilters.Filters.Add (new AutomaticRetryAttribute { Attempts = 5 });



#endregion
#region Services Registration
var config = builder.Configuration;

builder.Services.AddPersistence (config);
//Hải - Đăng ký tất cả các dịch vụ cần thiết của tầng Application
builder.Services.AddApplication ( );
builder.Services.AddInfrastructure (config);

builder.Services.AddHttpContextAccessor ( );
//Hải
builder.Services.AddControllers ( )
    .AddJsonOptions (options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // tùy chọn, để JSON đẹp
    });

builder.Services.AddSignalR ( );
builder.Services.AddScoped<IRealTimeNotificationPublisher, NotificationHubService> ( );

builder.Services.AddScoped<ICurrentUserService, CurrentUserService> ( );
builder.Services.AddScoped<IUserPermissionInfoService, UserPermissionInfoService> ( );

builder.Services.AddHealthChecks ( );

// Đăng ký Localization - chuẩn khi dùng Shared.Resources
builder.Services.AddLocalization (options => options.ResourcesPath = "Resources");

// Đăng ký IStringLocalizerFactory - để support cross-assembly (Shared.Resources)
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory> ( );

// Đăng ký ILocalizationService - Application Layer sẽ dùng qua interface này
builder.Services.AddScoped<ILocalizationService, LocalizationService> ( );
builder.Services.AddScoped<ISoftDeleteService, SoftDeleteService> ( );

// Cấu hình RequestLocalizationOptions
builder.Services.Configure<RequestLocalizationOptions> (options =>
{
    var supportedCultures = new[] { "en-US", "vi-VN", "vi", "en" };

    options.SetDefaultCulture ("en-US")
           .AddSupportedCultures (supportedCultures)
           .AddSupportedUICultures (supportedCultures);
});

builder.Services.AddEndpointsApiExplorer ( );
builder.Services.AddSwaggerGen ( );

builder.Services.AddOpenApiDocument (configure =>
{
    configure.Title = "Regal Education Portal API";

    configure.AddSecurity ("JWT", Enumerable.Empty<string> ( ), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}.",
    });

    configure.OperationProcessors.Add (new AspNetCoreOperationSecurityScopeProcessor ("JWT"));


});
builder.Services.Configure<JwtTokenConfig> (builder.Configuration.GetSection ("JwtTokenConfig"));
builder.Services.AddSingleton (sp => sp.GetRequiredService<IOptions<JwtTokenConfig>> ( ).Value);

string secret = config["JWTTokenConfig:Secret"] ?? throw new InvalidOperationException ("Missing Secret");
string issuer = config["JWTTokenConfig:Issuer"] ?? throw new InvalidOperationException ("Missing Issuer");
string audience = config["JWTTokenConfig:Audience"] ?? throw new InvalidOperationException ("Missing Audience");

builder.Services.AddAuthentication (x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer (x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (secret)),
        ValidAudience = audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var auth = context.Request.Headers["Authorization"].FirstOrDefault ( );
            if (!string.IsNullOrEmpty (auth))
            {
                context.Token = auth.Split (' ').Last ( );
                return Task.CompletedTask;
            }

            // Support SignalR access_token query param for hubs
            var accessToken = context.Request.Query["access_token"].FirstOrDefault ( );
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty (accessToken) && path.StartsWithSegments ("/hubs/notifications"))
            {
                context.Token = accessToken;
                return Task.CompletedTask;
            }

            if (context.Request.Path.StartsWithSegments ("/hangfire"))
            {
                var cookieToken = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty (cookieToken))
                    context.Token = cookieToken;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddSingleton (new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = issuer,
    ValidateAudience = true,
    ValidAudience = audience,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (secret)),
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
});
// Đăng ký Background Task Queue và Background Worker Service
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue> ( );
builder.Services.AddHostedService<BackgroundWorkerService> ( );  // Đăng ký Background Worker

builder.Services.Configure<PagingOptions> (builder.Configuration.GetSection ("Paging"));
builder.Services.AddSingleton (sp => sp.GetRequiredService<IOptions<PagingOptions>> ( ).Value);
builder.Services.AddScoped<IAdmissionsQuotaStatusJob, AdmissionsQuotaStatusJob> ( );
builder.Services.AddScoped<IClassStatusJob, ClassStatusJob> ( );// Hải thêm 22.11.2025
builder.Services.AddScoped<IClassScheduleStatusJob, ClassScheduleStatusJob> ( );// Hải thêm 03.12.2025
builder.Services.AddScoped<IClassAttendanceLockingJob, ClassAttendanceLockingJob> ( );// Hải thêm 04.12.2025
builder.Services.AddScoped<IClassScheduleUsableAmountJob, ClassScheduleUsableAmountJob> ( );// Hải thêm 04.12.2025
#region email configuration
builder.Services.Configure<EmailSettings> (builder.Configuration.GetSection ("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService> ( );
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService> ( );

#endregion
#region File Service
builder.Services.AddScoped<IFileService, FileService> ( );
#endregion
#region CORS
var validDomains = builder.Configuration.GetSection ("ValidDomains").Get<string[]> ( ) ?? Array.Empty<string> ( );

builder.Services.AddCors (options =>
{
    options.AddPolicy ("RegalEdu", cors =>
    {
        cors.WithOrigins (validDomains)
            .AllowAnyHeader ( )
            .AllowAnyMethod ( )
            .AllowCredentials ( );
    });
});
#endregion
builder.Services.AddHttpClient ( );
#endregion
// QuestPDF
QuestPDF.Settings.License = LicenseType.Community;
FontManager.RegisterFont (File.OpenRead ("Assets/Fonts/TimesNewRoman/times.ttf"));    // Regular
FontManager.RegisterFont (File.OpenRead ("Assets/Fonts/TimesNewRoman/timesbd.ttf"));  // Bold
FontManager.RegisterFont (File.OpenRead ("Assets/Fonts/TimesNewRoman/timesi.ttf"));   // Italic
FontManager.RegisterFont (File.OpenRead ("Assets/Fonts/TimesNewRoman/timesbi.ttf"));  // Bold Italic
builder.Services.AddScoped<IOutputCommitmentPdfService, OutputCommitmentPdfService> ( );

#region App Build
var app = builder.Build ( );
var tz = TimeZoneInfo.FindSystemTimeZoneById ("SE Asia Standard Time");

#endregion

#region Auto Migrate + Seed
using (var scope = app.Services.CreateScope ( ))
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>> ( );

    try
    {
        var dbContext = services.GetRequiredService<RegalEducationDbContext> ( );

        if (dbContext.Database.IsSqlServer ( ))
        {
            dbContext.Database.Migrate ( );
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>> ( );
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>> ( );
        await RoleSeeder.SeedRoles (roleManager);
        await UserSeeder.SeedAdminUser (userManager);
    }
    catch (Exception ex)
    {
        Log.Fatal (ex, "An error occurred during migration or seeding");
        throw;
    }
}
#endregion

#region Middleware Pipeline
app.UseForwardedHeaders (new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseMiddleware<CaptureRequestIpMiddleware> ( );

//app.UseSerilogRequestLogging();


app.UseMiddleware<GlobalExceptionMiddleware> ( );

app.UseHttpsRedirection ( );

var supportedCultures = new[] { "en-US", "vi-VN" };

var localizationOptions = new RequestLocalizationOptions ( )
    .SetDefaultCulture ("en-US")
    .AddSupportedCultures (supportedCultures)
    .AddSupportedUICultures (supportedCultures);

app.UseRequestLocalization (localizationOptions);

app.UseHealthChecks ("/health");
//app.UseHangfireDashboard ("/hangfire");
var basicOpts = new BasicAuthAuthorizationFilterOptions
{
    RequireSsl = app.Environment.IsProduction ( ),
    SslRedirect = app.Environment.IsProduction ( ),
    LoginCaseSensitive = true,
    Users = new[]
    {
        new BasicAuthAuthorizationUser
        {
            Login = builder.Configuration["Hangfire:Dashboard:User"],
            PasswordClear = builder.Configuration["Hangfire:Dashboard:Pass"]
        }
    }
};
app.UseRouting ( );
app.UseCors ("RegalEdu");

app.UseAuthentication ( );
app.UseAuthorization ( );

app.MapHub<NotificationHub> ("/hubs/notifications");

app.UseHangfireDashboard ("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new BasicAuthAuthorizationFilter(basicOpts),
        // Tuỳ chọn: kèm thêm filter tự viết để allowlist IP
        // new HangfireIpAllowlist(new [] { "127.0.0.1", "::1" })
    },
    DisplayStorageConnectionString = false,
    // Tuỳ chọn: chỉ cho view (đọc) khi cần
    // IsReadOnlyFunc = _ => true
});
app.MapControllers ( );

app.UsePiplineMiddleware ( );

app.UseOpenApi ( );
app.UseSwaggerUI (c =>
{
    c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Regal Education Portal API V1");
});
#endregion
RegalEdu.Application.Common.Logging.LoggerExtensions.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor> ( );

app.UseStaticFiles (new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider (
        Path.Combine (builder.Environment.WebRootPath, "images")),
    RequestPath = "/images"
});
app.UseStaticFiles (new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider (
        Path.Combine (builder.Environment.WebRootPath, "temp")),
    RequestPath = "/temp"
});
app.UseStaticFiles (new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider (
        Path.Combine (builder.Environment.WebRootPath, "supporting-documents")),
    RequestPath = "/supporting-documents"
});
//0:00 ngày 05 hàng tháng
RecurringJob.AddOrUpdate<IAdmissionsQuotaStatusJob> (
    "admissionsquota.roll-monthly-status",
    "maintenance",
   j => j.RollMonthlyStatusesAsync (CancellationToken.None),
    "0 0 5 * *",
    new RecurringJobOptions
    {
        TimeZone = tz
    }
);

//Hải thêm 22.11.2025
#region Hangfire Recurring Job - ClassStatusJob
// Tạo job chạy hàng ngày lúc 0:00
RecurringJob.AddOrUpdate<IClassStatusJob> (
    "class.roll-daily-status", // Id của job
    j => j.RollDailyStatusesAsync (CancellationToken.None), // Method cần chạy
    "0 0 * * *", // Cron expression: 0h00 mỗi ngày
    new RecurringJobOptions
    {
        TimeZone = tz
    }
);
#endregion

//Hải thêm 30.11.2025
#region Hangfire Recurring Job - ClassScheduleStatusJob (Tự động Hoàn thành buổi học cũ)
// Tạo job chạy hàng ngày lúc 0:00 sáng
RecurringJob.AddOrUpdate<IClassScheduleStatusJob> (
    "classschedule.complete-past-schedules", // Id duy nhất của job
    j => j.CompletePastSchedulesAsync (CancellationToken.None), // Phương thức cần chạy
    "0 0 * * *", // Cron expression: 0:00 AM mỗi ngày
    new RecurringJobOptions
    {
        TimeZone = tz // Sử dụng múi giờ Việt Nam
    }
);
#endregion
//Hải thêm 04.12.2025
#region Hangfire Recurring Job - ClassAttendanceLockingJob (Tự động Khóa điểm danh)
// Tạo job chạy hàng ngày lúc 0:00 sáng
RecurringJob.AddOrUpdate<IClassAttendanceLockingJob> (
    "classschedule.lock-past-attendance", // Id duy nhất mới cho job khóa điểm danh
    j => j.LockPastScheduleAttendanceAsync (CancellationToken.None), // Phương thức cần chạy
    "0 0 * * *", // Cron expression: 0:00 AM mỗi ngày
    new RecurringJobOptions
    {
        TimeZone = tz // Sử dụng múi giờ Việt Nam
    }
);
#endregion

#region App Run
app.Run ( );
#endregion
