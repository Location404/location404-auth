using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentity.Domain.Entities;

namespace UserIdentity.Infra.Context.EntityMapping;

public class PasswordAuthenticationMap : IEntityTypeConfiguration<PasswordAuthentication>
{
    public void Configure(EntityTypeBuilder<PasswordAuthentication> builder)
    {
        builder.ToTable("PasswordAuthentication");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PasswordSalt)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.RefreshToken)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(x => x.RefreshTokenExpirationDate)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastLoginAt)
            .IsRequired();

        builder.HasOne(x => x.UserApplication)
            .WithMany()
            .HasForeignKey(x => x.UserApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}