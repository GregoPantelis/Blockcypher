using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Migrations;

public static class DatabaseMigrationExtensions
{
    public static void MigrateDatabases(this IServiceProvider services)
    {
        using (IServiceScope scope = services.CreateScope())
        {
            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            UserAuthDbContext userDb = scope.ServiceProvider.GetRequiredService<UserAuthDbContext>();
            BlockcypherDbContext blockchainDb = scope.ServiceProvider.GetRequiredService<BlockcypherDbContext>();

            userDb.Database.Migrate();
            blockchainDb.Database.Migrate();

            DatabaseSeeder.Seed(userDb, configuration);
        }
    }
}