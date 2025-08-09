using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentityService.Domain.Entities;
using UserIdentityService.Domain.ValueObjects;

namespace UserIdentityService.Infrastructure.Context.EntitiesMapping;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => EmailAddress.Create(value))
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Username)
            .HasColumnName("username")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(u => u.ProfileImage)
            .HasColumnType("bytea")
            .HasColumnName("profile_image")
            .HasMaxLength(512);

        builder.Property(u => u.PreferredLanguage)  
            .HasColumnName("preferred_language")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.EmailVerified)
            .HasColumnName("email_verified")
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at")
            .IsRequired(false);

        builder.HasMany(u => u.ExternalLogins)
            .WithOne(el => el.User)
            .HasForeignKey(el => el.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_users_external_logins");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email");
    }
}