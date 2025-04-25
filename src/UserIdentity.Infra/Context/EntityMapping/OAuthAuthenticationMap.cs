using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context.EntityMapping;

public class OAuthAuthenticationMap : IEntityTypeConfiguration<OAuthAuthentication>
{
    public void Configure(EntityTypeBuilder<OAuthAuthentication> builder)
    {
        builder.ToTable("OAuthAuthentication");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Provider)
            .HasColumnName("Provider")
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.ProviderId)
            .HasColumnName("ProviderId")
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.RefreshToken)
            .HasColumnName("RefreshToken")
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(x => x.ExpirationDate)
            .HasColumnName("ExpirationDate")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(x => x.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .IsRequired();

        builder.HasOne(x => x.UserApplication).WithOne()
            .HasForeignKey<OAuthAuthentication>(x => x.UserApplicationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}