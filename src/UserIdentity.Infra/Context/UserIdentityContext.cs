using Microsoft.EntityFrameworkCore;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context;

public class UserIdentityContext(DbContextOptions<DbContext> options) : DbContext(options)
{
    public DbSet<UserApplication> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserIdentityContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}