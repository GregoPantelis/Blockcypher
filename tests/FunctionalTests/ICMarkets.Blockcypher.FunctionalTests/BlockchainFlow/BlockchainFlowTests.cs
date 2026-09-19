using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Tests.Shared.Fixtures;
using ICMarkets.Blockcypher.Tests.Shared.Common;
using Xunit;

namespace ICMarkets.Blockcypher.FunctionalTests.BlockchainFlow;

public class BlockchainFlowTests : IClassFixture<BlockcypherWebApplicationFactory>
{
    private readonly BlockcypherWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authenticationHelper; 
    
    public BlockchainFlowTests(BlockcypherWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticationHelper = new AuthenticationHelper(_factory);
        _client = _authenticationHelper.HttpClient;
    }

    [Fact]
    public async Task AuthenticatedUser_CanCaptureBlockchainSnapshot()
    {
        await _authenticationHelper.AuthenticateAsync();
        
        BlockchainSnaphotRequest request = GlobalUsings.ValidBlockchainSnaphotRequest;
        
        DateTime before = DateTime.Now;
        
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        var responseContent = await response.Content.ReadFromJsonAsync<BlockchainSnapshotResponse>();
        
        DateTime after = DateTime.Now;
        
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        
        Assert.True(!string.IsNullOrEmpty(responseContent.Chain));
        Assert.True(!string.IsNullOrEmpty(responseContent.Coin));
        Assert.True(!string.IsNullOrEmpty(responseContent.BlockchainData.ToString()));
        
        Assert.InRange(responseContent.CreateAt, before, after);
    }

    [Fact]
    public async Task CapturedBlockchainSnapshot_AppearsInHistory()
    {
        await _authenticationHelper.AuthenticateAsync();
        
        BlockchainSnaphotRequest request = GlobalUsings.ValidBlockchainSnaphotRequest;
        var snapshot = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        BlockchainSnapshotResponse snapshotResponse = await snapshot.Content.ReadFromJsonAsync<BlockchainSnapshotResponse>();
        
        Assert.NotNull(snapshot);
        Assert.NotNull(snapshotResponse);
        Assert.NotNull(snapshotResponse.BlockchainData);
        
        HttpResponseMessage response = await _client.GetAsync($"/api/blockchains/{request.Coin.ToLower()}/{request.Chain.ToLower()}/history");
        BlockchainHistoryResponse responseContent = await response.Content.ReadFromJsonAsync<BlockchainHistoryResponse>();
        
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.BlockchainHistoryData);

        Assert.Contains(
            responseContent.BlockchainHistoryData,
            item =>
                item?["Hash"]?.ToString() ==
                snapshotResponse.BlockchainData?["Hash"]?.ToString()
                &&
                item?["Height"]?.GetValue<long>() ==
                snapshotResponse.BlockchainData?["Height"]?.GetValue<long>());
    }

    [Fact]
    public async Task UnauthenticatedUser_CannotAccessBlockchainFeatures()
    {
        BlockchainSnaphotRequest request = GlobalUsings.ValidBlockchainSnaphotRequest;
        
        HttpResponseMessage snapshot = await _client.PostAsJsonAsync("/api/blockchains/snapshot", request);
        HttpResponseMessage history = await _client.GetAsync($"/api/blockchains/{request.Coin.ToLower()}/{request.Chain.ToLower()}/history");
        
        Assert.Equal(HttpStatusCode.Unauthorized, snapshot.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, history.StatusCode);
    }
}