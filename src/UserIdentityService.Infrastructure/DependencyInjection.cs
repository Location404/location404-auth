using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserIdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUserIdentityInfra(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<Context.UserIdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("UserIdentityDatabase");
            options.UseNpgsql(connectionString);

            // Enable sensitive data logging
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        return services;
    }
}