using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using System.Linq.Expressions;

namespace ICMarkets.Blockcypher.Application.Interfaces.Persistence
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        
        Task<OperationResult<bool>> AddAsync(TEntity entity);

        Task<OperationResult<bool>> DeleteAsync(TEntity entity);

        Task<OperationResult<bool>> UpdateAsync(TEntity entity);

    }
}
