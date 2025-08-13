using LiteBus.Commands.Extensions.MicrosoftDependencyInjection;
using LiteBus.Messaging.Extensions.MicrosoftDependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using UserIdentityService.Application.Features.UserManagement.Commands.CreateUserWithPasswordCommand;
using UserIdentityService.Infrastructure.Services;

namespace UserIdentityService.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services required by the User Identity module:
    /// - Configures Entity Framework Core DbContext (UserIdentityDbContext) to use PostgreSQL with the "UserIdentityDatabase" connection string,
    ///   enabling sensitive data logging and detailed errors.
    /// - Adds LiteBus and registers the command module by scanning the assembly containing CreateUserWithPasswordHandler.
    /// - Registers the CustomExceptionHandler for centralized exception handling.
    /// </summary>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddUserIdentityInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context.UserIdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("UserIdentityDatabase");
            options.UseNpgsql(connectionString);

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        services.AddLiteBus(liteBus =>
        {
            liteBus.AddCommandModule(module => module.RegisterFromAssembly(typeof(CreateUserWithPasswordHandler).Assembly));
            // liteBus.AddQueryModule(module => module.RegisterFromAssembly(typeof(CreateUserWithExternalProviderHandler).Assembly));
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}