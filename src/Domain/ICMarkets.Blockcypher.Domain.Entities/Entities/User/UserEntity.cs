namespace ICMarkets.Blockcypher.Domain.Entities.User;

public class UserEntity
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public bool IsActive { get; set; }
}