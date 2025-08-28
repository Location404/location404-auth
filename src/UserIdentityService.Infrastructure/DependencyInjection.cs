using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using UserIdentityService.Application.Common.Interfaces;
using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;
using UserIdentityService.Infrastructure.Services;

namespace UserIdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUserIdentityInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context.UserIdentityDbContext>(options =>
        {
            #if DEBUG
            var connectionString = configuration.GetConnectionString("UserIdentityDatabaseDevelopment");
            #else
            var connectionString = configuration.GetConnectionString("UserIdentityDatabaseProduction");
            #endif
            options.UseNpgsql(connectionString);

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        services.AddLiteBus(liteBus =>
        {
            liteBus.AddCommandModule(module => module.RegisterFromAssembly(typeof(CreateUserWithPasswordCommandHandler).Assembly));
            // liteBus.AddQueryModule(module => module.RegisterFromAssembly(typeof(CreateUserWithExternalProviderHandler).Assembly));
        });

        services.AddScoped<IEncryptPasswordService, BcryptEncryptPasswordService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}