using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration.DbConnections
{
    public sealed class ConnectionStringOptions
    {
        public const string SectionName = "DatabaseConnections";

        [Required]
        [ValidateEnumeratedItems]
        public IEnumerable<ConnectionString> ConnectionStrings { get; set; }

    }
}
