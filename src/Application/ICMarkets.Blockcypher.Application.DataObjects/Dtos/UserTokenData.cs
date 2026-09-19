namespace ICMarkets.Blockcypher.Application.DataObjects.Dtos;

public class UserTokenData
{
    public string Username { get; set; }
    
    public string Token { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime ExpiresAtUtc { get; set; }
}