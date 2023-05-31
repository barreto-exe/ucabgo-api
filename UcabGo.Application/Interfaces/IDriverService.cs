using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Inputs;

namespace UcabGo.Application.Interfaces
{
    public interface IDriverService
    {
        Task<RideDto> CreateRide(RideInput input);
        Task<RideDto> StartRide(RideAvailableInput input);
        Task<RideDto> CompleteRide(RideAvailableInput input);
        Task<RideDto> CancelRide(RideAvailableInput input);

        Task<PassengerDto> AcceptPassenger(string driverEmail, int rideId, int passengerId);
        Task<PassengerDto> IgnorePassenger(string driverEmail, int rideId, int passengerId);
        Task<PassengerDto> CancelPassenger(string driverEmail, int rideId, int passengerId);
    }
}
