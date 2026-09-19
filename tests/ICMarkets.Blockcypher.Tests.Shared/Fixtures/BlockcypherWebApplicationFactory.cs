using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;

namespace ICMarkets.Blockcypher.Tests.Shared.Fixtures;

public class BlockcypherWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"blockcypher-integration-{Guid.NewGuid()}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove production DbContext registrations
            services.RemoveAll<BlockcypherDbContext>();
            services.RemoveAll<UserAuthDbContext>();
            
            services.RemoveAll<DbContextOptions<BlockcypherDbContext>>();
            services.RemoveAll<DbContextOptions<UserAuthDbContext>>();
            
            // Both contexts use the SAME SQLite DB, matching the production architecture.
            var connectionString = $"Data Source={_databasePath}";

            services.AddDbContext<BlockcypherDbContext>(options =>
            {
                options.UseSqlite(
                    connectionString,
                    x => x.MigrationsHistoryTable(
                        "__EFMigrationsHistory_Blockchain"));
            });

            services.AddDbContext<UserAuthDbContext>(options =>
            {
                options.UseSqlite(
                    connectionString,
                    x => x.MigrationsHistoryTable(
                        "__EFMigrationsHistory_UserAuth"));
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            base.Dispose(disposing);
            SqliteConnection.ClearAllPools();
            
            DeleteIfExists(_databasePath);
            DeleteIfExists($"{_databasePath}-shm");
            DeleteIfExists($"{_databasePath}-wal");
        }
    }
    
    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        using IServiceScope scope =host.Services.CreateScope();

        var userContext = scope.ServiceProvider.GetRequiredService<UserAuthDbContext>();

        var blockchainContext = scope.ServiceProvider.GetRequiredService<BlockcypherDbContext>();

        userContext.Database.Migrate();
        blockchainContext.Database.Migrate();

        DatabaseSeeder.SeedUser(userContext);
        
        return host;
    }
}