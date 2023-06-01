using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Vehicle> VehicleRepository { get; }
        IRepository<Soscontact> SoscontactRepository { get; }
        IRepository<Destination> DestinationRepository { get; }
        IRepository<Location> LocationRepository { get; }
        IRepository<Passenger> PassengerRepository { get; }
        IRepository<Ride> RideRepository { get; }
        IRepository<Chatmessage> ChatmessageRepository { get; }
        IRepository<Evaluation> EvaluationRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
