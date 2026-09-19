using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Mappers;

public static class UserAuthModelMapper
{
    public static UserAuthData ToUserAuthData(UserModel model, UserAuthModel authModel)
    {
        if (model == null || authModel == null)
        {
            return null;
        }

        return new UserAuthData()
        {
            UserId = model.Id,
            Username = model.Username,
            PasswordHash = authModel.PasswordHash,
            CreatedAt = model.CreatedAt,
            UtcCreatedAt = model.UtcCreatedAt,
            UpdatedAt = model.UpdatedAt,
            IsActive = model.IsActive,
        };
    }
}