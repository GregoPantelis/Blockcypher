using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Requests;

public class LoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [JsonPropertyName("password")]
    public string Password { get; set; }
}