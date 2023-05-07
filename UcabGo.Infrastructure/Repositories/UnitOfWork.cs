using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;

namespace UcabGo.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UcabgoContext ucabgoContext;
        private readonly IRepository<User> userRepository;
        public UnitOfWork(UcabgoContext ucabgoContext)
        {
            this.ucabgoContext = ucabgoContext;
        }


        public IRepository<User> UserRepository => userRepository ?? new BaseRepository<User>(ucabgoContext);


        public void Dispose()
        {
            if (ucabgoContext != null)
            {
                ucabgoContext.Dispose();
            }
        }
        public void SaveChanges()
        {
            ucabgoContext.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await ucabgoContext.SaveChangesAsync();
        }
    }
}
