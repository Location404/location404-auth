using Microsoft.EntityFrameworkCore;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context;

public class UserIdentityContext(DbContextOptions<UserIdentityContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.UserName).IsRequired().HasMaxLength(12);
        modelBuilder.Entity<User>().Property(u => u.EmailAddress).IsRequired().HasMaxLength(64);
        modelBuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
        modelBuilder.Entity<User>().Property(u => u.LastLogin).IsRequired(false);

        modelBuilder.Entity<User>().Property(u => u.IsActive).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.CreatedAt).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.UpdatedAt).IsRequired();

        // modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        // modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                entry.CurrentValues["CreatedAt"] = DateTime.UtcNow;
                entry.CurrentValues["UpdatedAt"] = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.CurrentValues["UpdatedAt"] = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}