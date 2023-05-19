using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;

namespace UcabGo.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UcabgoContext ucabgoContext;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Vehicle> vehicleRepository;
        private readonly IRepository<Soscontact> soscontactRepository;

        public UnitOfWork(UcabgoContext ucabgoContext)
        {
            this.ucabgoContext = ucabgoContext;
        }

        public IRepository<User> UserRepository => userRepository ?? new BaseRepository<User>(ucabgoContext);
        public IRepository<Vehicle> VehicleRepository => vehicleRepository ?? new BaseRepository<Vehicle>(ucabgoContext);
        public IRepository<Soscontact> SoscontactRepository => soscontactRepository ?? new BaseRepository<Soscontact>(ucabgoContext);

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
