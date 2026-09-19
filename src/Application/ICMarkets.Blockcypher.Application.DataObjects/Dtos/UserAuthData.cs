using ICMarkets.Blockcypher.Application.DataObjects.Enums;

namespace ICMarkets.Blockcypher.Application.DataObjects.Dtos;

public class UserAuthData
{
    public long UserId { get; set; }

    public string Username { get; set; }
    
    public string PasswordHash { get; set; } = null!;
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime UtcCreatedAt { get; set; }
}