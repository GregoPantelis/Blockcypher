using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMarkets.Blockcypher.Application.DataObjects.Constants
{
    public static class ConfigurationConstants
    {
        public static class ConnectionStringNames
        {
            public const string DefaultDb = "Default";
            public const string BlockcypherDb = "BlockcypherDb";
        }

        public static class JwtTokenIssuers
        {
            public const string DefaultIssuer = "Blockcypher";
            public const string BlockcypherIssuer = DefaultIssuer;
        }
    }
}
