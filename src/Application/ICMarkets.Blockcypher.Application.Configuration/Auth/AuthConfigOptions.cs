using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Application.Configuration.Auth;

public class AuthConfigOptions
{
    public const string SectionName = "AuthSettings";
    
    [Required]
    public string TokenType { get; set; }
    
    [Required]
    [ValidateEnumeratedItems]
    public IEnumerable<JwtAuthConfig> JwtAuthConfigs { get; set; }
}