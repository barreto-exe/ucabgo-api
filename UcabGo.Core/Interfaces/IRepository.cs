using System.Linq.Expressions;
using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncluding(Expression<Func<T, object>> includeProperty);
        Task<T> GetById(object id);
        Task Add(T item);
        void Update(T item);
        Task Delete(object id);
    }
}
