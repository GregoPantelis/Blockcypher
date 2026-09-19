using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.EFConfigurations
{
    internal sealed class BlockchainModelConfiguration : IEntityTypeConfiguration<BlockchainModel>
    {
        public void Configure(EntityTypeBuilder<BlockchainModel> builder)
        {
            builder.ToTable("Blockchains");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Coin)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.Chain)
                .IsRequired()
                .HasMaxLength(10);
            builder.Property(b => b.BlockchainData)
                .HasColumnType("TEXT")
                .IsRequired(false);
            builder.Property(b => b.CreatedAt)
                .IsRequired();
            builder.Property(b => b.UpdatedAt)
                .IsRequired();
            builder.Property(b => b.UtcCreatedAt)
                .IsRequired();
        }
    }
}
