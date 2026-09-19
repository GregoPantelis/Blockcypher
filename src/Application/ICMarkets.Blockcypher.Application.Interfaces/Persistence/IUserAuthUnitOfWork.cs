using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Persistence;

public interface IUserAuthUnitOfWork : IUnitOfWork
{
    Task<OperationResult<UserAuthData>> GetUserAsync(UserDataFilter filter, CancellationToken cancellationToken = default);
    
}