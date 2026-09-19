using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMarkets.Blockcypher.Domain.Entities
{
    public partial class BlockCypherBase
    {
        public string Chain { get; set; }

        public string Coin { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UtcCreatedAt { get; set; }
    }
}
