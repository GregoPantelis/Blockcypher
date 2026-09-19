using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Application.Services.Blockchain;
using ICMarkets.Blockcypher.Application.Services.Authentication;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Application.DataObjects.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockcypher.Application.DependencyInjection
{
    public static class ApplicationCollectionExtensions
    {
        public static IServiceCollection AddBlockcypherApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices();
            services.AddAuthConfigSettings(configuration);
            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBlockchainService, BlockchainService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            return services;
        }
        
        private static IServiceCollection AddAuthConfigSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptionsWithValidateOnStart<AuthConfigOptions>()
                .Bind(configuration.GetSection(AuthConfigOptions.SectionName))
                .ValidateDataAnnotations();
            
            // check the Token Type here and add the appropriate authentication mechanism
            string tokenType = configuration.GetSection(AuthConfigOptions.SectionName)[nameof(AuthConfigOptions.TokenType)];;
            if (string.IsNullOrWhiteSpace(tokenType))
            {
                throw new  InvalidOperationException($"{AuthConfigOptions.SectionName} token type is not configured.");
            }
            
            services.AddAuthentication();
            services.AddAuthorization();
            
            switch (tokenType.ToUpperInvariant())
            {
                case "JWT":
                    services.AddJwtAuthentication();
                    break;

                default:
                    throw new NotSupportedException(
                        $"Authentication token type '{tokenType}' is not supported.");
            }
            
            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<AuthConfigOptions>>((jwtOptions, authOptions) =>
                {
                    JwtAuthConfig config = authOptions.Value.JwtAuthConfigs
                        .FirstOrDefault(x => x.Issuer.Equals(ConfigurationConstants.JwtTokenIssuers.BlockcypherIssuer));
                    
                    if (config == null) 
                        throw new InvalidOperationException($"{ConfigurationConstants.JwtTokenIssuers.BlockcypherIssuer} is not configured.");

                    jwtOptions.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = config.Issuer,
                            ValidAudience = config.Audience,

                            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(config.Signature))
                        };
                });
            
            return services;
        }
    }
}
