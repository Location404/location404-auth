using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Common.Interfaces;
using UserIdentity.Application.Features.Authentication.Commands.RegisterUser;
using UserIdentity.Application.Features.Authentication.Interfaces;
using UserIdentity.Application.Features.UserManagement;
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

    private static void AddUserIdentityAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));

        var googleLoginSettings = configuration.GetSection(GoogleLoginSettings.SectionName).Get<GoogleLoginSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings, nameof(googleLoginSettings));

        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        })
        .AddGoogle(options =>
        {
            options.ClientId = googleLoginSettings!.ClientId;
            options.ClientSecret = googleLoginSettings.ClientSecret;
            options.CallbackPath = "/signin-google";
        });
    }


    private static void AddUserIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
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
    }

    private static void AddUserIdentityServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddMediatR(typeof(RegisterUserCommand).Assembly);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddExceptionHandler<CustomExceptionHandler>();
    }
    
    private static void AddUserIdentitySettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<GoogleLoginSettings>(configuration.GetSection(GoogleLoginSettings.SectionName));
    }
}