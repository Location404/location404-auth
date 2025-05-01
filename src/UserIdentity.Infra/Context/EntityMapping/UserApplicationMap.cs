using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context.EntityMapping;

public class UserApplicationMap : IEntityTypeConfiguration<UserApplication>
{
    public void Configure(EntityTypeBuilder<UserApplication> builder)
    {
        builder.ToTable("user_application");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Username)
            .HasColumnName("username")
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.EmailAddress)
            .HasColumnName("email_address")
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ProfilePictureUrl)
            .HasColumnName("profile_pictureUrl")
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(x => x.PreferredLanguage)
            .HasColumnName("preferred_language")
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(x => x.GoogleId)
            .HasColumnName("google_id")
            .IsRequired(false)
            .HasMaxLength(128);

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PasswordSalt)
            .HasColumnName("password_salt")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.RefreshToken)
            .HasColumnName("refresh_token")
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(x => x.RefreshTokenExpiryTime)
            .HasColumnName("refresh_token_expiry_time")
            .IsRequired(false);

        // indexes
        builder.HasIndex(x => x.Username)
            .HasDatabaseName("idx_user_application_username")
            .IsUnique();

        builder.HasIndex(x => x.EmailAddress)
            .HasDatabaseName("idx_user_application_email_address")
            .IsUnique();

        builder.HasIndex(x => x.GoogleId)
            .HasDatabaseName("idx_user_application_google_id")
            .IsUnique();
    }
}