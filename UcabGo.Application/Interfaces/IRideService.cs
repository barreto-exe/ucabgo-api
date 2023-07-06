using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IRideService
    {
        Task<IEnumerable<RideMatchDto>> GetMatchingAll(MatchingFilter filter);
        Task<IEnumerable<RideDto>> GetAll(string driverEmail, bool onlyActive = false);
        Task<IEnumerable<Ride>> GetAll();
        Task<Ride> GetById(int id);
        Task<IEnumerable<PassengerDto>> GetPassengers(int rideId);
        Task<(IEnumerable<RideDto>, IEnumerable<string>)> CancelInactiveRides();
        Task<IEnumerable<User>> GetUsers(Ride ride);
        Task<IEnumerable<UserDto>> GetUsers(RideDto ride);
    }
}
