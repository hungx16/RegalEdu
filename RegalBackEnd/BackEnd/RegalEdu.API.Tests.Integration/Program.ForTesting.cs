using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using RegalEdu.Application;
using System.Text;
using RegalEdu.Infrastructure;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Infrastructure.Services;
using RegalEdu.Application.Common.Settings;
using Microsoft.Extensions.Options;
using RegalEdu.Infrastructure.Identity;
using Microsoft.Extensions.Localization;
using FluentValidation;
using MediatR;
using RegalEdu.Application.Common.Behaviors;
using RegalEdu.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RegalEdu.BackEnd.Middleware;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing"
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

Console.WriteLine($"[ProgramForTesting] Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddValidatorsFromAssemblyContaining<ILocalizationService>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

builder.Services.AddTestPersistence();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(RegalEdu.Api.Controllers.UserController).Assembly)
    .AddControllersAsServices();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserPermissionInfoService, UserPermissionInfoService>();

builder.Services.AddHealthChecks();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "Regal Education Portal API - TESTING";
    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}.",
    });
    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

// JWT config
builder.Services.Configure<JwtTokenConfig>(builder.Configuration.GetSection("JwtTokenConfig"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtTokenConfig>>().Value);

var jwtSection = builder.Configuration.GetSection("JwtTokenConfig");

Console.WriteLine($"[ProgramForTesting] JWT Secret: {jwtSection["Secret"]}");

string secret = jwtSection["Secret"] ?? throw new InvalidOperationException("Missing Secret");
string issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing Issuer");
string audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Missing Audience");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
        ValidAudience = audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// CORS
var validDomains = builder.Configuration.GetSection("ValidDomains").Get<string[]>() ?? Array.Empty<string>();

Console.WriteLine("[ProgramForTesting] CORS Valid Domains:");
foreach (var domain in validDomains)
{
    Console.WriteLine($"- {domain}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("RegalEdu", cors =>
    {
        cors.WithOrigins(validDomains)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

// 🚀 FIX THỨ TỰ MIDDLEWARE CHUẨN NHẤT:
app.UseRouting();

app.UseCors("RegalEdu"); // 🚀 CORS phải sau UseRouting

app.UseAuthentication(); // 🚀 Phải sau UseRouting
app.UseAuthorization();  // 🚀 Phải sau UseAuthentication

app.MapControllers();    // 🚀 MapControllers phải cuối → guaranteed không 404

app.UsePiplineMiddleware();

app.UseOpenApi();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Regal Education Portal API V1 - TESTING");
});

app.Run();
var controllerAssemblies = typeof(RegalEdu.Api.Controllers.UserController).Assembly.FullName;
Console.WriteLine($"[ProgramForTesting] Controller Assembly: {controllerAssemblies}");

namespace RegalEdu.Api
{
    public partial class ProgramForTesting { }
}
