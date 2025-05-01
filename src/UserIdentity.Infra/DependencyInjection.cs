using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddDbContext<UserIdentityContext>(op =>
        {
            op.UseNpgsql(configuration.GetConnectionString("UserIdentityDb"));

#if DEBUG
            op.EnableSensitiveDataLogging();
            op.EnableDetailedErrors();
#endif
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        return services;
    }
}