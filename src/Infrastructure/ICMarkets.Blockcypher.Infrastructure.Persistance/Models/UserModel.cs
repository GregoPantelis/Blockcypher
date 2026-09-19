using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Models;

public class UserModel
{
    [Required]
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public DateTime UtcCreatedAt { get; set; }
}