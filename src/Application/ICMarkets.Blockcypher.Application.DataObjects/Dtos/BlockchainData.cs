using Newtonsoft.Json.Linq;

namespace ICMarkets.Blockcypher.Application.DataObjects.Dtos
{
    public class BlockchainData
    {
        public string Chain { get; set; }

        public string Coin { get; set; }

        public JObject BChainData {  get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UtcCreatedAt { get; set; }
    }
}
