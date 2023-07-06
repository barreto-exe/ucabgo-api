using Newtonsoft.Json;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;

namespace UcabGo.Core.Data.Ride.Dtos
{
    public class RideDto : ISignalRDto
    {
        public int Id { get; set; }
        public UserDto Driver { get; set; }
        public VehicleDto Vehicle { get; set; }
        public LocationDto Destination { get; set; }
        public int SeatQuantity { get; set; }
        public int AvailableSeats
        {
            get
            {
                var activePassengers = Passengers.Where(p =>
                    p.TimeAccepted != null &&
                    p.TimeCancelled == null &&
                    p.TimeIgnored == null &&
                    p.TimeFinished == null);
                return SeatQuantity - activePassengers.Count();
            }
        }
        public float LatitudeOrigin { get; set; }
        public float LongitudeOrigin { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime? TimeStarted { get; set; }
        public DateTime? TimeEnded { get; set; }
        public DateTime? TimeCanceled { get; set; }
        public virtual IEnumerable<PassengerDto> Passengers { get; set; }

        [JsonIgnore]
        public IEnumerable<string> UsersToMessage { get; set; }
    }
}
