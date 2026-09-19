using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ICMarkets.Blockcypher.Api.Contracts.Responses
{
    public class BlockchainHistoryResponse
    {
        [JsonPropertyName("coin")]
        public string Coin { get; set; }

        [JsonPropertyName("chain")]
        public string Chain { get; set; }

        [JsonPropertyName("blockchainHistoryData")]
        public List<JsonNode> BlockchainHistoryData { get; set; }
    }
}
