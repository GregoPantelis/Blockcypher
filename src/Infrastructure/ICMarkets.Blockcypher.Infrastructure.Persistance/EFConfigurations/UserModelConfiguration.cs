using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.EFConfigurations;

public class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(pk => pk.Id);
        builder.Property(p => p.Username)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        builder.Property(p => p.UtcCreatedAt)
            .IsRequired();
    }
}