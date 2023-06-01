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
        private readonly IRepository<Destination> destinationRepository;
        private readonly IRepository<Location> locationRepository;
        private readonly IRepository<Passenger> passengerRepository;
        private readonly IRepository<Ride> rideRepository;
        private readonly IRepository<Chatmessage> chatmessageRepository;

        public UnitOfWork(UcabgoContext ucabgoContext)
        {
            this.ucabgoContext = ucabgoContext;
        }

        public IRepository<User> UserRepository => userRepository ?? new BaseRepository<User>(ucabgoContext);
        public IRepository<Vehicle> VehicleRepository => vehicleRepository ?? new BaseRepository<Vehicle>(ucabgoContext);
        public IRepository<Soscontact> SoscontactRepository => soscontactRepository ?? new BaseRepository<Soscontact>(ucabgoContext);
        public IRepository<Destination> DestinationRepository => destinationRepository ?? new BaseRepository<Destination>(ucabgoContext);
        public IRepository<Location> LocationRepository => locationRepository ?? new BaseRepository<Location>(ucabgoContext);
        public IRepository<Passenger> PassengerRepository => passengerRepository ?? new BaseRepository<Passenger>(ucabgoContext);
        public IRepository<Ride> RideRepository => rideRepository ?? new BaseRepository<Ride>(ucabgoContext);
        public IRepository<Chatmessage> ChatmessageRepository => chatmessageRepository ?? new BaseRepository<Chatmessage>(ucabgoContext);

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
