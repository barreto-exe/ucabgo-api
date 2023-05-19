using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T> GetById(object id);
        Task Add(T item);
        void Update(T item);
        Task Delete(object id);
    }
}
