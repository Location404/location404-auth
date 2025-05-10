using System.Text;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Application.Features.UserManagement;
using UserIdentity.Application.Features.UserManagement.Commands.RegisterUser;
using UserIdentity.Infra.Context;
using UserIdentity.Infra.Persistence;
using UserIdentity.Infra.Services;
using UserIdentity.Infra.Settings;

namespace UserIdentity.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddUserIdentityInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUserIdentitySettings(configuration);
        services.AddUserIdentityDbContext(configuration);
        services.AddUserIdentityServices();
        services.AddUserIdentityAuthentication(configuration);

        return services;
    }

    public static IServiceCollection AddUserIdentityAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));

        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                RequireExpirationTime = true,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }


    public static IServiceCollection AddUserIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserIdentityContext>(op =>
        {
#if DEBUG
            op.UseNpgsql(configuration.GetConnectionString("UserIdentityDb")).LogTo(Console.WriteLine, LogLevel.Information);
            op.EnableSensitiveDataLogging();
            op.EnableDetailedErrors();
#else
            op.UseNpgsql(configuration.GetConnectionString("UserIdentityDb"));
#endif
        });

        return services;
    }

    public static IServiceCollection AddUserIdentityServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }


    public static IServiceCollection AddUserIdentitySettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        return services;
    }
}