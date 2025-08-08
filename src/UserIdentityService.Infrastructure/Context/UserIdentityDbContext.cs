using Microsoft.EntityFrameworkCore;
using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Infrastructure.Context;

public class UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ExternalLogin> ExternalLogins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfiguration(new UserMapping());
        // modelBuilder.ApplyConfiguration(new ExternalLoginMapping());

        // get from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}