using Newtonsoft.Json;
using static ICMarkets.Blockcypher.Application.DataObjects.Helpers.JsonResolvers;

namespace ICMarkets.Blockcypher.Domain.Entities
{
    public class BlockchainEntity : BlockCypherBase
    {
        public string Name { get; set; }

        public long Height { get; set; }

        public string Hash { get; set; }

        public DateTimeOffset Time { get; set; }

        public string LatestUrl { get; set; }

        public string PreviousHash { get; set; }

        public string PreviousUrl { get; set; }

        public int PeerCount { get; set; }

        public int UnconfirmedCount { get; set; }

        public long HighFeePerKb { get; set; }

        public long MediumFeePerKb { get; set; }

        public long LowFeePerKb { get; set; }

        public long LastForkHeight { get; set; }

        public string LastForkHash { get; set; }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new IgnoreBaseContractResolver()
            };

            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
