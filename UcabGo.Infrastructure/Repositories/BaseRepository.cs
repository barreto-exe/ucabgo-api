using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;

namespace UcabGo.Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> entities;
        public BaseRepository(UcabgoContext ucabgoContext)
        {
            this.entities = ucabgoContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return entities;
        }

        public IQueryable<T> GetAllIncluding(Expression<Func<T, object>> includeProperty)
        {
            return entities.Include(includeProperty);
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = entities;

            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty);
            }

            return query;
        }

        public async Task<T> GetById(object id)
        {
            return await entities.FindAsync(id);
        }

        public async Task<T> GetById(object id, params Expression<Func<T, object>>[] navigationProperties)
        {
            DbSet<T> query = entities;

            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty) as DbSet<T>;
            }

            return await query.FindAsync(id);
        }

        public async Task Add(T item)
        {
            await entities.AddAsync(item);
        }

        public void Update(T item)
        {
            entities.Update(item);
        }

        public async Task Delete(object id)
        {
            var currentEntity = await GetById(id);
            entities.Remove(currentEntity);
        }

    }
}
