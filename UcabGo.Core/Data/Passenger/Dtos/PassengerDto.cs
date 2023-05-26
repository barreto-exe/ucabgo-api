using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Passanger.Dtos
{
    public class PassengerDto
    {
        public int Id { get; set; }
        public virtual RideDto Ride { get; set; }
        public virtual LocationDto InitialLocation { get; set; }
        public virtual UserDto User { get; set; }
        public DateTime TimeSolicited { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public DateTime? TimeIgnored { get; set; }
        public DateTime? TimeCancelled { get; set; }
    }
}
