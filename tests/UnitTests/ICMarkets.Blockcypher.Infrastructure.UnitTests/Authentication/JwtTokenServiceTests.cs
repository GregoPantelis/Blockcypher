using System.IdentityModel.Tokens.Jwt;
using ICMarkets.Blockcypher.Infrastructure.Authentication.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarkets.Blockcypher.Infrastructure.UniTests.Authentication;

public class JwtTokenServiceTests
{
    private readonly Mock<ILogger<JwtTokenService>> _logger;
    private JwtTokenService _service;
    
    public JwtTokenServiceTests()
    {
        _logger = new Mock<ILogger<JwtTokenService>>();
        
        _service = new JwtTokenService(
            _logger.Object,
            GlobalUsings.AuthConfigOptions);
    }
    
    [Fact]
    public void GenerateTokenAsync_ValidUser_ReturnsToken()
    {
        var user = GlobalUsings.CreateUserAuth();
        var result = _service.GenerateInHouseJwtToken(user);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));

        var handler = new JwtSecurityTokenHandler();

        Assert.True(handler.CanReadToken(result.Data.Token));
    }

    [Fact]
    public void GenerateTokenAsync_ValidUser_ContainsExpectedClaims()
    {
        var user = GlobalUsings.CreateUserAuth();
        var result = _service.GenerateInHouseJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data.Token);
        
        Assert.Equal(GlobalUsings.Issuer, token.Issuer);
        Assert.Contains(GlobalUsings.Audience, token.Audiences);

        Assert.Equal(
            user.UserId.ToString(),
            token.Claims
                .First(x => x.Type == JwtRegisteredClaimNames.Sub)
                .Value);

        Assert.Equal(
            user.Username,
            token.Claims
                .First(x => x.Type == JwtRegisteredClaimNames.UniqueName)
                .Value);

        Assert.False(
            string.IsNullOrWhiteSpace(
                token.Claims
                    .First(x => x.Type == JwtRegisteredClaimNames.Jti)
                    .Value));
    }

    [Fact]
    public void GenerateTokenAsync_ValidConfiguration_SetsCorrectExpiry()
    {
        // Arrange
        var user = GlobalUsings.CreateUserAuth();
        var beforeGeneration = DateTime.Now;
        
        var result = _service.GenerateInHouseJwtToken(user);

        var afterGeneration = DateTime.Now;
        var expectedMinimumExpiry = beforeGeneration.AddMinutes(GlobalUsings.ExpiryMinutes);
        var expectedMaximumExpiry = afterGeneration.AddMinutes(GlobalUsings.ExpiryMinutes);

        Assert.InRange(
            result.Data.ExpiresAt,
            expectedMinimumExpiry,
            expectedMaximumExpiry);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data.Token);

        Assert.InRange(
            token.ValidTo,
            expectedMinimumExpiry.AddSeconds(-1).ToUniversalTime(),
            expectedMaximumExpiry.AddSeconds(1).ToUniversalTime());
    }
}