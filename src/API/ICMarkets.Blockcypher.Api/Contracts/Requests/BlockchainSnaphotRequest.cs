using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Requests
{
    public class BlockchainSnaphotRequest
    {
        [JsonPropertyName("chain")]
        public string Chain { get; set; }

        [JsonPropertyName("coin")]
        public string Coin { get; set; }
    }
}
