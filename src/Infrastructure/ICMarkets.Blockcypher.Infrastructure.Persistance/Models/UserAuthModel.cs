using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Models;

public class UserAuthModel
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public DateTime UtcCreatedAt { get; set; }
}