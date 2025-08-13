using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Infrastructure.Context.EntitiesMapping;

public class ExternalLoginMapping : IEntityTypeConfiguration<ExternalLogin>
{
    /// <summary>
    /// Configures the EF Core mapping for the <c>ExternalLogin</c> entity.
    /// </summary>
    /// <remarks>
    /// Maps the entity to the "external_logins" table, defines a composite primary key on
    /// <c>LoginProvider</c> and <c>ProviderKey</c>, configures column names and constraints for
    /// <c>LoginProvider</c> (required, max 50), <c>ProviderKey</c> (required, max 256) and
    /// <c>UserId</c> (required), and creates an index on <c>UserId</c> named
    /// "ix_external_logins_user_id".
    /// </remarks>
    /// <param name="builder">The EF Core entity type builder for configuring <c>ExternalLogin</c>.</param>
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable("external_logins");
        builder.HasKey(el => new { el.LoginProvider, el.ProviderKey });

        builder.Property(el => el.LoginProvider)
            .HasColumnName("login_provider")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(el => el.ProviderKey)
            .HasColumnName("provider_key")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(el => el.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(el => el.UserId)
            .HasDatabaseName("ix_external_logins_user_id");
    }
}