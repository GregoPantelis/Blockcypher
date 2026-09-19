using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Configuration.Apis
{
    public sealed class ExternalApi
    {
        /// <summary>
        /// The name of the external API. This property is used to identify the API in the configuration.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The base URL of the external API. This property is used to make requests to the API.
        /// </summary>
        [Required]
        public string BaseUrl { get; set; }

        public string ApiVersion { get; set; }

        /// <summary>
        /// The API key for the external API. This property is used to authenticate requests to the API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The API secret for the external API. This property is used to authenticate requests to the API.
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// The timeout for requests to the external API, in seconds. The default value is 60 seconds.
        /// </summary>
        public int Timeout{ get; set; } = 60;
    }
}
