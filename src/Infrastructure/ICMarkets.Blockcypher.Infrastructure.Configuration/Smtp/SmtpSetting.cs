
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration.Smtp
{
    public sealed class SmtpSetting
    {
        /// <summary>
        /// The name of the SMTP setting.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The SMTP host address.
        /// </summary>
        [Required]
        public string SmtpHost { get; set; }

        /// <summary>
        /// The SMTP port number.
        /// </summary>
        [Required]
        public string SmtpPort { get; set; }

        /// <summary>
        /// The ssl setting for the SMTP connection.
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
