using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration
{
    public abstract class ConfigSettingsBase
    {
        public abstract string SectionName { get; }
    }
}
