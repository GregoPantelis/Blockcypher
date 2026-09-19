using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Responses;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; }
    
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }
}