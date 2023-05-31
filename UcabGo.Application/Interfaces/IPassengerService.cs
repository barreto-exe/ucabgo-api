using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IPassengerService
    {
        Task<Passenger> GetById(int id);
        Task<PassengerDto> AskForRide(PassengerInput input);
        Task<IEnumerable<RideDto>> GetRides(RideFilter filter);
    }
}
