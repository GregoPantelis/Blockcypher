using ICMarkets.Blockcypher.Application.Interfaces.Persistence;
using ICMarkets.Blockcypher.Domain.Types.Common;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Repository
{
    public class Repository<TContext, TEntity> : IRepository<TEntity> 
        where TContext : CustomContext 
        where TEntity : class
    {
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        protected TContext Context { get; }

        protected DbSet<TEntity> DbSet { get; }

        public Repository(TContext context, ILogger<UnitOfWork<TContext>>  logger)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
            _logger  = logger;
        }

        #region Queries

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = DbSet.Where(predicate);
            return query;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = DbSet;
            return query;
        }

        #endregion

        #region CRUD Operations

        public async virtual Task<OperationResult<bool>> AddAsync(TEntity entity)
        {
            try
            {
                EntityEntry<TEntity> entry = await DbSet.AddAsync(entity);
                if (entry != null)
                {
                    return new OperationResult<bool>(OperationResults.Common.Successful, true);
                }

                return new OperationResult<bool>(OperationResults.Common.InvalidInput, false);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while adding new entity");
                return new OperationResult<bool>(OperationResults.Common.InternalServerError, false);
            }
        }

        public virtual Task<OperationResult<bool>> DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResult<bool>> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
