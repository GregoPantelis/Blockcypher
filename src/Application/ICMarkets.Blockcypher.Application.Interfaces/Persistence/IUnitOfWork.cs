using ICMarkets.Blockcypher.Domain.Types.OperationalResult;

namespace ICMarkets.Blockcypher.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Returns a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        /// <summary>
        /// Saves all changes made in this unit of work to the database.    
        /// </summary>
        /// <returns>Returns an OperationResult containing a boolean indicating the success of the operation.</returns>
        Task<OperationResult<bool>> CompleteAsync();
    }
}