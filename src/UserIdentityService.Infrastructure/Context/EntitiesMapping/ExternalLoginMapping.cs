using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentityService.Domain.Entities;

namespace UserIdentityService.Infrastructure.Context.EntitiesMapping;

public class ExternalLoginMapping : IEntityTypeConfiguration<ExternalLogin>
{
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