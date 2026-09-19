using ICMarkets.Blockcypher.Api.Helpers;
using ICMarkets.Blockcypher.Application.DependencyInjection;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Migrations;
using ICMarkets.Blockcypher.Infrastructure.DependencyInjection; 
using Serilog;

namespace ICMarkets.Blockcypher.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IHostEnvironment hostEnvironment = builder.Environment;
            
            // add config.json file to the configuration
            builder.Configuration
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"config.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            // Configure logging (serilog)
            builder.Configuration
                .AddJsonFile(
                    "log.config",
                    optional: false,
                    reloadOnChange: true);

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });
            
            // COnfigure Services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddBlockcypherInfrastructure(builder.Configuration, hostEnvironment.ContentRootPath);
            builder.Services.AddBlockcypherApplication(builder.Configuration);
            builder.Services.AddBlockcypherApi(builder.Configuration, hostEnvironment);
            
            var app = builder.Build();

            app.Services.MigrateDatabases();
            
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseCors(ServiceCollectionExtensions.CorsPolicy);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}

// For integration testing
public partial class Program { }
