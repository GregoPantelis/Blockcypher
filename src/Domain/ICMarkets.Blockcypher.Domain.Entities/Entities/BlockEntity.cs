
namespace ICMarkets.Blockcypher.Domain.Entities
{
    public class BlockEntity : BlockCypherBase
    {
        public string Hash { get; set; }
        public long Height { get; set; }
        public string Chain { get; set; }
        public long Total { get; set; }
        public long Fees { get; set; }
        public int Size { get; set; }
        public int Vsize { get; set; }
        public long Ver { get; set; }
        public DateTimeOffset Time { get; set; }
        public DateTimeOffset ReceivedTime { get; set; }
        public string CoinbaseAddr { get; set; }
        public string RelayedBy { get; set; }
        public long Bits { get; set; }
        public long Nonce { get; set; }
        public int NTx { get; set; }
        public string PrevBlock { get; set; }
        public string MrklRoot { get; set; }
        public List<string> Txids { get; set; }
        public int Depth { get; set; }
        public string PrevBlockUrl { get; set; }
        public string TxUrl { get; set; }
        public string NextTxids { get; set; }
    }
}
