using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts
{
    public sealed class BlockcypherDbContext : CustomContext
    {
        public DbSet<BlockchainModel> Blockchain { get; set;  }

        public BlockcypherDbContext(DbContextOptions<BlockcypherDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BlockcypherDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
