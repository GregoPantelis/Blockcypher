using System.Net;
using System.Net.Http.Json;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Tests.Shared.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ICMarkets.Blockcypher.IntegrationTests.Api;

public class AuthenticationTests : IClassFixture<BlockcypherWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly BlockcypherWebApplicationFactory _factory;

    public AuthenticationTests(
        BlockcypherWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public void Database_TestUserIsSeeded()
    {
        using var scope =
            _factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<UserAuthDbContext>();

        var user = context.Users
            .SingleOrDefault(x =>
                x.Username ==
                GlobalUsings.ValidLoginRequest.Username);

        Assert.NotNull(user);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = GlobalUsings.ValidLoginRequest;

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrWhiteSpace(loginResponse.Token));
    }
    
    [Fact]
    public async Task LoginAsync_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = GlobalUsings.InvalidLoginRequest;

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
    
    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = GlobalUsings.InvalidCredsLoginRequest;

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}