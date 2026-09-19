using System.Net;
using System.Net.Http.Json;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Tests.Shared.Fixtures;
using ICMarkets.Blockcypher.Tests.Shared.Common;
using Xunit;

namespace ICMarkets.Blockcypher.FunctionalTests.AuthenticationFlow;

public class AuthenticationFlowTests : IClassFixture<BlockcypherWebApplicationFactory>
{
    private readonly BlockcypherWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authenticationHelper; 
    
    public AuthenticationFlowTests(BlockcypherWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticationHelper = new AuthenticationHelper(_factory);
        _client = _authenticationHelper.HttpClient;
    }

    [Fact]
    public async Task User_CanLoginAndAccessProtectedEndpoint()
    {
        await _authenticationHelper.AuthenticateAsync();

        BlockchainSnaphotRequest request = GlobalUsings.ValidBlockchainSnaphotRequest;
        
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        var responseContent = await response.Content.ReadFromJsonAsync<BlockchainSnapshotResponse>();
        
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }
}