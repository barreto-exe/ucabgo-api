using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
