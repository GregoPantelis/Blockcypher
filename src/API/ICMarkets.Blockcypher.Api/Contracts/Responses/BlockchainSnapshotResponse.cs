using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Responses
{
    public class BlockchainSnapshotResponse
    {
        [JsonPropertyName("coin")]
        public string Coin { get; set; }

        [JsonPropertyName("chain")]
        public string Chain { get; set; }

        [JsonPropertyName("blockchainData")]
        public JsonNode BlockchainData { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreateAt { get; set; }
        
        [JsonPropertyName("utcCreatedAt")]
        public DateTime UtcCreatedAt{ get; set; }
    }
}
