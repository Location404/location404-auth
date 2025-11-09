using System.Text;
using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;
using LiteBus.Queries.Extensions.MicrosoftDependencyInjection;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Location404.Auth.Application.Common.Interfaces;
using Location404.Auth.Application.Features.Authentication.Commands.AuthenticateUserWithPasswordCommand;
using Location404.Auth.Application.Features.Authentication.Interfaces;
using Location404.Auth.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;
using Location404.Auth.Application.Features.UserManagement.Queries.GetCurrentUserInformation;
using Location404.Auth.Infrastructure.Services;
using Location404.Auth.Infrastructure.Settings;

namespace Location404.Auth.Infrastructure;

public static class DependencyInjection
{
    private static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    public static IServiceCollection AddUserIdentityInfra(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddMessageBus(services);
        AddApplicationServices(services);
        AddJwtAuthentication(services, configuration);
        AddAuthorizationPolicies(services);
        AddCorsConfiguration(services, configuration);

        return services;
    }

    private static void AddCorsConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var originsString = configuration.GetValue<string>("Cors:AllowedOrigins");
        var allowedOrigins = originsString?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = IsDevelopment
            ? configuration.GetConnectionString("UserIdentityDatabaseDevelopment")
            : configuration.GetConnectionString("UserIdentityDatabaseProduction");

        services.AddDbContext<Context.UserIdentityDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }

    private static void AddMessageBus(IServiceCollection services)
    {
        services.AddLiteBus(liteBus =>
        {
            liteBus.AddCommandModule(module => module.RegisterFromAssembly(typeof(CreateUserWithPasswordCommandHandler).Assembly));
            liteBus.AddCommandModule(module => module.RegisterFromAssembly(typeof(AuthenticateUserWithPasswordCommandHandler).Assembly));
            liteBus.AddQueryModule(module => module.RegisterFromAssembly(typeof(GetCurrentUserInformationQueryHandler).Assembly));
        });
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IEncryptPasswordService, BcryptEncryptPasswordService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>().Bind(configuration.GetSection(nameof(JwtSettings)));

        JwtSettings jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                ConfigureJwtBearerOptions(options, jwtSettings, IsDevelopment);

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (IsDevelopment)
                        {
                            var keyValuePair = new KeyValuePair<string, StringValues>("Token-Error", context.Exception.Message);
                            context.Response.Headers.Append(keyValuePair);
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, JwtSettings jwtSettings, bool isDevelopment)
    {
        options.RequireHttpsMetadata = !isDevelopment;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),

            RequireExpirationTime = true,
            RequireSignedTokens = true,

            RequireAudience = true,
            ValidateActor = false,
            ValidateTokenReplay = false
        };
    }

    private static void AddAuthorizationPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Administrator"));
        });
    }
}