using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Passenger.Dtos;
using UcabGo.Core.Data.Passenger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IPassengerService
    {
        Task<Passenger> GetById(int id);
        Task<IEnumerable<RideDto>> GetRides(RideFilter filter);
        Task<PassengerDto> AskForRide(PassengerInput input);
        Task<PassengerDto> CancelRide(CancelRideInput input);
        Task<PassengerDto> FinishRide(FinishRideInput input);
        Task<CooldownDto> GetPassengerCooldownTime(string email);
    }
}
