using Ingenious.Hackathon.Docusign.Sql.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ingenious.Hackathon.Docusign.Sql
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private HackathonDbContext _moneyPalDbContext;
        private DbSet<T> _dbSet;
        private readonly IServiceScopeFactory _scopeFactory;

        public Repository(IServiceScopeFactory scopeFactory, HackathonDbContext moneyPalDbContext)
        {
            _dbSet = moneyPalDbContext.Set<T>();
            _moneyPalDbContext=moneyPalDbContext;
            _scopeFactory = scopeFactory;
        }

        private HackathonDbContext CreateNewContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<HackathonDbContext>();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                var entity = await _dbSet.FindAsync(id);
                return entity;
            });
        }

        public async Task<List<T>> FindByAsync(RepositoryModel<T> repositoryModel)
        {
            var dbSet = NoTrackingOptOut(repositoryModel.AsNoTrackingOptOut).Where(repositoryModel.Where);
            if (repositoryModel.Include != null) dbSet = repositoryModel.Include(dbSet);

            return await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                var query = repositoryModel.OrderBys.Any()
                    ? dbSet.OrderByWithExpressionTransform(repositoryModel.OrderBys)
                    : dbSet;

                var projectedResult = query.ToList();

                return projectedResult;
            });
        }

        public async Task<List<TResult>> FindByAsync<TResult>(RepositoryModel<T, TResult> repositoryModel, bool? isEncrypted = false)
        {
            var dbSet = NoTrackingOptOut(repositoryModel.AsNoTrackingOptOut).Where(repositoryModel.Where);

            if (repositoryModel.Include != null)
                dbSet = repositoryModel.Include(dbSet);

            return await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                var query = repositoryModel.OrderBys.Any()
                    ? dbSet.OrderByWithExpressionTransform(repositoryModel.OrderBys)
                    : dbSet;

                // Apply the select projection
                var projectedResult = await query.Select(repositoryModel.Select).ToListAsync();

                return projectedResult;
            });
        }

        public async Task<PagedEntities<T>> FindByPageAsync(RepositoryModel<T> repositoryModel)
        {
            if (!repositoryModel.OrderBys.Any())
            {
                repositoryModel.OrderBys.Add(new OrderByModel<T> { Ascending = true });
            }
            var dbSet = NoTrackingOptOut(repositoryModel.AsNoTrackingOptOut).Where(repositoryModel.Where);
            if (repositoryModel.Include != null) dbSet = repositoryModel.Include(dbSet);
            var dbSetOrdered = dbSet.OrderByWithExpressionTransform(repositoryModel.OrderBys);
            var count = await dbSetOrdered.CountAsync();
            var query = dbSetOrdered.Skip(repositoryModel.Skip).Take(repositoryModel.Take);
            // Decrypt each entity in the result if encryption is needed

            var items = new List<T>();
            await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                items = await query.ToListAsync();
            });

            return new PagedEntities<T>
            {
                Count = count,
                Entities = items
            };
        }

        public async Task<T> GetAsync(RepositoryModel<T> repositoryModel)
        {
            if (repositoryModel.Id != null && repositoryModel.Include == null)
            {
                var data = await _dbSet.FindAsync(repositoryModel.Id);
                return data;
            }

            //if (repositoryModel.Id != null)
            //{
            //    repositoryModel.Where = x => x.Id == repositoryModel.Id;
            //}

            return await RetryHelper.RetryOnExceptionAsync(async () =>
            {
                var dbSet = NoTrackingOptOut(repositoryModel.AsNoTrackingOptOut).Where(repositoryModel.Where);
                if (repositoryModel.Include != null) dbSet = repositoryModel.Include(dbSet);
                if (!repositoryModel.OrderBys.Any())
                {
                    var entity = dbSet.FirstOrDefault();
                    return entity;
                }
                var result = await dbSet.OrderByWithExpressionTransform(repositoryModel.OrderBys).FirstOrDefaultAsync();
                return result;
            });
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _moneyPalDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _moneyPalDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _moneyPalDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _moneyPalDbContext.SaveChangesAsync();
            }
        }

        internal IQueryable<T> NoTrackingOptOut(bool optOut)
        {
            _moneyPalDbContext = CreateNewContext();
            _dbSet = _moneyPalDbContext.Set<T>();
            return optOut ? _moneyPalDbContext.Set<T>() : _moneyPalDbContext.Set<T>().AsNoTracking();
        }
    }
}
