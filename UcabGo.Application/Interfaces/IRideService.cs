using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.Ride.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IRideService
    {
        Task<IEnumerable<RideDto>> GetMathchingAll(MatchingFilter filter);
        Task<IEnumerable<RideDto>> GetAll(string driverEmail, bool onlyActive = false);
        Task<IEnumerable<Ride>> GetAll();
        Task<Ride> GetById(int id);
        Task<IEnumerable<PassengerDto>> GetPassengers(int rideId);
    }
}
