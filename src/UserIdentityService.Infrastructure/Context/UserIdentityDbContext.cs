using Microsoft.EntityFrameworkCore;
using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Infrastructure.Context;

public class UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ExternalLogin> ExternalLogins { get; set; }

    /// <summary>
    /// Configures the EF Core model for this context by applying IEntityTypeConfiguration implementations found in the containing assembly.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings; this method applies all configurations from the assembly that contains <see cref="UserIdentityDbContext"/>.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfiguration(new UserMapping());
        // modelBuilder.ApplyConfiguration(new ExternalLoginMapping());

        // get from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}