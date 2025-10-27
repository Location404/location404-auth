using Microsoft.EntityFrameworkCore;
using Location404.Auth.Domain.Entities;

namespace Location404.Auth.Infrastructure.Context;

public class UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ExternalLogin> ExternalLogins { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}