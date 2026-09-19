using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Application.Configuration.Auth;

public class JwtAuthConfig
{
    [Required]
    public string Signature { get; set; }
    
    [Required]
    public string Issuer { get; set; }
    
    [Required]
    public string Audience { get; set; }

    [Range(1, 1440)]
    public int ExpiryMinutes { get; set; } = 60;
}