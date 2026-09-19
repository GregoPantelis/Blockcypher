using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Mappers;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;

public class UserAuthUnitOfWork : UnitOfWork<UserAuthDbContext>, IUserAuthUnitOfWork
{
    private readonly ILogger<UserAuthUnitOfWork> _logger;
    private readonly AuthConfigOptions _authConfigOptions;

    public UserAuthUnitOfWork(
        UserAuthDbContext context,
        IOptions<AuthConfigOptions> authConfigOptions,
        ILogger<UserAuthUnitOfWork> logger)
        : base(context, logger)
    {
        _logger = logger;
        _authConfigOptions = authConfigOptions.Value;
    }

    public async Task<OperationResult<UserAuthData>> GetUserAsync(UserDataFilter filter, CancellationToken cancellationToken = default)
    {
        try
        {
            UserModel user = await this.Repository<UserModel>()
                .FindBy(x => x.Username.Equals(filter.Username))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return new OperationResult<UserAuthData>(OperationResults.UserAuth.UserNotFound, null);

            UserAuthModel userAuth = await this.Repository<UserAuthModel>()
                .FindBy(x => x.UserId == user.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (userAuth == null)
            {
                _logger.LogError($"User auth does not exist for userid: {user.Id}");
                return  new OperationResult<UserAuthData>(OperationResults.Common.InvalidInput, null);
            }

            UserAuthData userAuthData = UserAuthModelMapper.ToUserAuthData(user, userAuth);
            
            return userAuthData == null
                    ? new OperationResult<UserAuthData>(OperationResults.UserAuth.UserNotFound, null)
                    : new OperationResult<UserAuthData>(OperationResults.Common.Successful, userAuthData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user data");
            return new OperationResult<UserAuthData>(OperationResults.Common.InternalServerError, null);
        }
    }
}