using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts
{
    public class CustomContext : DbContext
    {
        public CustomContext(DbContextOptions options)
            : base(options)
        {
            
        }
    }
}
