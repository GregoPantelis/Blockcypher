using ICMarkets.Blockcypher.Tests.Shared.Fixtures;
using System.Net;
using System.Net.Http.Json;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Tests.Shared.Common;
using Xunit;

namespace ICMarkets.Blockcypher.IntegrationTests.Api;

public class BlockchainTests : IClassFixture<BlockcypherWebApplicationFactory>
{
    private readonly BlockcypherWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authenticationHelper; 

    public BlockchainTests(BlockcypherWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticationHelper = new AuthenticationHelper(_factory);
        _client = _authenticationHelper.HttpClient;
    }
    
    [Fact]
    public async Task GetBlockchainData_WithoutToken_ReturnsUnauthorized()
    {
        var request = GlobalUsings.ValidBlockchainSnaphotRequest;

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
    
    [Fact]
    public async Task GetHistory_WithoutToken_ReturnsUnauthorized()
    {
        HttpResponseMessage response = await _client.GetAsync( "/api/blockchains/btc/main/history");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBlockchainData_InvalidRequest_ReturnsBadRequest()
    {
        await _authenticationHelper.AuthenticateAsync();

        var request = GlobalUsings.InvalidLoginRequest;

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
    }
    
    [Fact]
    public async Task GetHistory_InvalidRequest_ReturnsBadRequest()
    {
        await _authenticationHelper.AuthenticateAsync();

        string coin = "Empty";
        string chain = "invalid-chain";
        
        HttpResponseMessage response = await _client.GetAsync($"/api/blockchains/{coin}/{chain}/history");
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(errorResponse);
        Assert.Contains("INVALID", errorResponse.ErrorMessage, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("INVALID", errorResponse.Error, StringComparison.CurrentCultureIgnoreCase);
    }
    
    [Fact]
    public async Task GetHistory_ValidRequest_ReturnsOk()
    {
        await _authenticationHelper.AuthenticateAsync();

        string coin = Coin.BTC.ToString();
        string chain = Chain.Main.ToString();

        BlockchainSnaphotRequest request = new BlockchainSnaphotRequest()
        {
            Coin = coin,
            Chain = chain,
        };

        await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        
        HttpResponseMessage response = await _client.GetAsync($"/api/blockchains/{coin}/{chain}/history");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetHistory_ValidRequest_NoDbData_ReturnsNotFound()
    {
        await _authenticationHelper.AuthenticateAsync();

        string coin = Coin.BTC.ToString();
        string chain = Chain.Main.ToString();
        
        HttpResponseMessage response = await _client.GetAsync($"/api/blockchains/{coin}/{chain}/history");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBlockchainData_ValidRequest_ReturnsOk()
    {
        await _authenticationHelper.AuthenticateAsync();

        var request = GlobalUsings.ValidBlockchainSnaphotRequest;

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        var responseContent = await response.Content.ReadFromJsonAsync<BlockchainSnapshotResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }
}