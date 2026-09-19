using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration.Smtp
{
    public sealed class SmtpSettingsOptions
    {
        public const string SectionName = "SmtpSettings";

        [ValidateEnumeratedItems]
        public IEnumerable<SmtpSetting> Settings { get; set; }
    }
}
