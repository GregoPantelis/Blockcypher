namespace ICMarkets.Blockcypher.Domain.Entities.User;

public class UserAuth
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string PasswordHash { get; set; } = null!;
}