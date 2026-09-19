using FluentValidation;
using ICMarkets.Blockcypher.Api.Validators;

namespace ICMarkets.Blockcypher.Api.Helpers;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicy = "DefaultCorsPolicy";
    
    public static IServiceCollection AddBlockcypherApi(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        services.AddRequestValidators(configuration);
        services.AddLogging(configuration);
        services.AddControllers(hostEnvironment);
        services.AddCorsPolicy();
        services.AddHealthCheckEndpoints();
        return services;
    }

    private static IServiceCollection AddRequestValidators(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<BlockchainSnapshotRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<BlockchainHistoryRequestValidator>();
        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration);
        });
        return services;
    }

    private static IServiceCollection AddControllers(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        services.AddControllersWithViews();
        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        return services;
    }

    private static IServiceCollection AddHealthCheckEndpoints(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}