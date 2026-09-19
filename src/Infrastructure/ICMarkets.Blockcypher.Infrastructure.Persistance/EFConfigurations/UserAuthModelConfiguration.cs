using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ICMarkets.Blockcypher.Infrastructure.Persistance.EFConfigurations;

public class UserAuthModelConfiguration : IEntityTypeConfiguration<UserAuthModel>
{
    public void Configure(EntityTypeBuilder<UserAuthModel> builder)
    {
        builder.ToTable("UserAuth");
        builder.HasKey(pk => pk.Id);
        builder.Property(p => p.UserId)
            .IsRequired();
        builder.Property(p => p.PasswordHash)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        builder.Property(p => p.UtcCreatedAt)
            .IsRequired();
    }
}