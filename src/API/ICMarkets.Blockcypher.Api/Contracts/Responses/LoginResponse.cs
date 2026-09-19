using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Responses;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
    
    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}