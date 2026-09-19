
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration.DbConnections
{
    public sealed class ConnectionString
    {
        [Required]
        public string DatabaseName { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
