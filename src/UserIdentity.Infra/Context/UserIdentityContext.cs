using Microsoft.EntityFrameworkCore;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context;

public class UserIdentityContext(DbContextOptions<DbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    // public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        # region [Entity Configurations]

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PasswordSalt).IsRequired();
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.EmailAddress).IsUnique();
            entity.HasIndex(e => e.DisplayName).IsUnique();
        });

        # endregion

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}