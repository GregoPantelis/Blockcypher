using ICMarkets.Blockcypher.Application.DataObjects.Enums;

namespace ICMarkets.Blockcypher.Application.DataObjects.Filters
{
    public class BlockchainDataFilter
    {
        public Coin Coin { get; set; }

        public Chain Chain { get; set; }

        public DateTime DateTimeFrom { get; set; }

        public DateTime DateTimeTo { get; set; }
    }
}
