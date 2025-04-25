using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context.EntityMapping;

public class UserApplicationMap : IEntityTypeConfiguration<UserApplication>
{
    public void Configure(EntityTypeBuilder<UserApplication> builder)
    {
        builder.ToTable("UserApplication");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasColumnName("DisplayName")
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Username)
            .HasColumnName("Username")
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.EmailAddress)
            .HasColumnName("EmailAddress")
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ProfilePictureUrl)
            .HasColumnName("ProfilePictureUrl")
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(x => x.PreferredLanguage)
            .HasColumnName("PreferredLanguage")
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired(false);
    }
}