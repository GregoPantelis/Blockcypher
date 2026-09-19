using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Constants;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using Microsoft.IdentityModel.Tokens;

namespace ICMarkets.Blockcypher.Infrastructure.Authentication.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly AuthConfigOptions _configOptions;
    
    public JwtTokenService(ILogger<JwtTokenService> logger, IOptions<AuthConfigOptions> configOptions)
    {
        _logger = logger;
        _configOptions =  configOptions.Value;
    }

    /// <inheritdoc/>
    public OperationResult<UserTokenData> GenerateInHouseJwtToken(UserAuthData userAuthData, CancellationToken cancellationToken = default)
    {
        try
        {
            JwtAuthConfig jwtConfig = null;
            if (!IsJwtAuthConfiguredForInHouse(out jwtConfig))
            {
                _logger.LogError($"Auth configuration is not set for {AuthToken.Jwt} for Inhouse Blockcypher Issuer token generation ");
                return new OperationResult<UserTokenData>(OperationResults.UserAuth.UserAuthNotSupported, null);
            }
            
            DateTime issuedAtUtc = DateTime.UtcNow;
            DateTime issuedAt = DateTime.Now;
            DateTime expiresAtUtc = issuedAtUtc.AddMinutes(jwtConfig.ExpiryMinutes);
            DateTime expiresAt = issuedAt.AddMinutes(jwtConfig.ExpiryMinutes);
            
            List<Claim> claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userAuthData.UserId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userAuthData.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            SymmetricSecurityKey key = new SymmetricSecurityKey(Convert.FromBase64String(jwtConfig.Signature));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                notBefore: issuedAt,
                expires: expiresAt,
                signingCredentials: credentials);
            
            string tokenValue = new JwtSecurityTokenHandler()
                .WriteToken(token);

            UserTokenData tokenData = new UserTokenData()
            {
                Username = userAuthData.Username,
                Token = tokenValue,
                ExpiresAt = expiresAt,
                CreatedAt = issuedAt,
                ExpiresAtUtc = expiresAtUtc,
                CreatedAtUtc = issuedAtUtc
            };
            
            return new OperationResult<UserTokenData>(OperationResults.Common.Successful, tokenData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating jwt token");
            return new OperationResult<UserTokenData>(OperationResults.Common.InternalServerError, null);
        }
    }

    private bool IsJwtAuthConfiguredForInHouse(out JwtAuthConfig jwtConfig)
    {
        // Always retrieve BlockcypherIssuer for inhouse token generation
        jwtConfig = _configOptions.JwtAuthConfigs
            .FirstOrDefault(x => x.Issuer.Equals(ConfigurationConstants.JwtTokenIssuers.BlockcypherIssuer));
        
        return jwtConfig != null
               && _configOptions.TokenType.Equals(AuthToken.Jwt.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}