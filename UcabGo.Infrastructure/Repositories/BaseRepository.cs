using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<T>> GetAll()
        {
            var foo = await entities.ToListAsync();
            return foo;
        }

        public async Task<T> GetById(object id)
        {
            return await entities.FindAsync(id);
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
