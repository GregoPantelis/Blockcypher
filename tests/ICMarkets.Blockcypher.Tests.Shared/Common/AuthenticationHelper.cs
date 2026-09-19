using System.Net.Http.Headers;
using System.Net.Http.Json;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Tests.Shared.Fixtures;

namespace ICMarkets.Blockcypher.Tests.Shared.Common;

public class AuthenticationHelper
{
    private readonly BlockcypherWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public HttpClient HttpClient => _client;
    
    public AuthenticationHelper(BlockcypherWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    public async Task AuthenticateAsync()
    {
        string token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        var request = GlobalUsings.ValidLoginRequest;

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.EnsureSuccessStatusCode();

        LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrWhiteSpace(loginResponse.Token));

        return loginResponse.Token;
    }
}