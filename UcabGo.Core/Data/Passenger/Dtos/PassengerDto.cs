using Newtonsoft.Json;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Passanger.Dtos
{
    public class PassengerDto : ISignalRDto
    {
        public int Id { get; set; }
        public virtual int Ride { get; set; }
        public virtual LocationDto FinalLocation { get; set; }
        public virtual UserDto User { get; set; }
        public DateTime TimeSolicited { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public DateTime? TimeIgnored { get; set; }
        public DateTime? TimeCancelled { get; set; }
        public DateTime? TimeFinished { get; set; }

        [JsonIgnore]
        public IEnumerable<string> UsersToMessage { get; set; }
    }
}
