using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Application.DataObjects.Constants;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Infrastructure.Configuration.Apis;
using ICMarkets.Blockcypher.Infrastructure.Configuration.DbConnections;
using ICMarkets.Blockcypher.Infrastructure.Configuration.Smtp;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.ApiClients;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.ApiServices;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;
using ICMarkets.Blockcypher.Infrastructure.Authentication.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockcypher.Infrastructure.DependencyInjection
{
    public static class InfrastructureCollectionExtensions
    {
        public static IServiceCollection AddBlockcypherInfrastructure(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.AddConnectionStrings(configuration);
            services.AddDatabaseContexts(contentRootPath);
            services.AddUnitOfWork();
            services.AddExternalApiSettings(configuration);
            services.AddInfrastructureServices();
            services.AddSmtpSettings(configuration);
            return services;
        }

        private static IServiceCollection AddConnectionStrings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptionsWithValidateOnStart<ConnectionStringOptions>()
                .Bind(configuration.GetSection(ConnectionStringOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(options => options.ConnectionStrings.Any(x => 
                    string.Equals(x.DatabaseName, ConfigurationConstants.ConnectionStringNames.DefaultDb, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(x.Value)),
                $"{ConfigurationConstants.ConnectionStringNames.DefaultDb} connection string is not configured.");

            return services;
        }

        private static IServiceCollection AddDatabaseContexts(this IServiceCollection services, string contentRootPath)
        {
            #region BlockcypherDbContext
            
            services.AddDbContext<BlockcypherDbContext>((provider, options) =>
            {
                ConnectionStringOptions settings = provider.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;

                ConnectionString connectionString = settings.ConnectionStrings.SingleOrDefault(x => 
                    string.Equals(x.DatabaseName, ConfigurationConstants.ConnectionStringNames.BlockcypherDb, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(connectionString?.Value))
                {
                    throw new InvalidOperationException($"{ConfigurationConstants.ConnectionStringNames.BlockcypherDb} connection string is not configured.");
                }

                var connectionBuilder = new SqliteConnectionStringBuilder(connectionString.Value);

                // Rooted path for database so it is constant regardless of where the application is running from
                if (!Path.IsPathRooted(connectionBuilder.DataSource))
                {
                    connectionBuilder.DataSource = Path.GetFullPath(
                        connectionBuilder.DataSource,
                        contentRootPath);
                }

                string directory = Path.GetDirectoryName(connectionBuilder.DataSource);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                options.UseSqlite(connectionBuilder.ConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory_Blockchain"));
            });

            #endregion
            
            #region UserAuthDbContext
            
            services.AddDbContext<UserAuthDbContext>((provider, options) =>
            {
                ConnectionStringOptions settings = provider.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;

                ConnectionString connectionString = settings.ConnectionStrings.SingleOrDefault(x => 
                    string.Equals(x.DatabaseName, ConfigurationConstants.ConnectionStringNames.BlockcypherDb, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(connectionString?.Value))
                {
                    throw new InvalidOperationException($"{ConfigurationConstants.ConnectionStringNames.BlockcypherDb} connection string is not configured.");
                }

                var connectionBuilder = new SqliteConnectionStringBuilder(connectionString.Value);

                // Rooted path for database so it is constant regardless of where the application is running from
                if (!Path.IsPathRooted(connectionBuilder.DataSource))
                {
                    connectionBuilder.DataSource = Path.GetFullPath(
                        connectionBuilder.DataSource,
                        contentRootPath);
                }

                string directory = Path.GetDirectoryName(connectionBuilder.DataSource);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                options.UseSqlite(connectionBuilder.ConnectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory_UserAuth"));
            });

            #endregion

            // Any other DbContexts can be added here in a similar manner
            return services;
        }

        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IBlockcypherUnitOfWork, BlockcypherUnitOfWork>();
            services.AddScoped<IUserAuthUnitOfWork, UserAuthUnitOfWork>();
            return services;
        }

        private static IServiceCollection AddExternalApiSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptionsWithValidateOnStart<ExternalApiOptions>()
                .Bind(configuration.GetSection(ExternalApiOptions.SectionName))
                .ValidateDataAnnotations();

            services.AddHttpClient();
            services.AddScoped<IApiClient, ApiClient>();

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IBlockchainDataProvider, BlockchainDataProvider>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            return services;
        }

        private static IServiceCollection AddSmtpSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptionsWithValidateOnStart<SmtpSettingsOptions>()
                .Bind(configuration.GetSection(SmtpSettingsOptions.SectionName))
                .ValidateDataAnnotations();

            return services;
        }
    }
}
