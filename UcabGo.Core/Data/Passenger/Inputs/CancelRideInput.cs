using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Passenger.Inputs
{
    public class CancelRideInput : BaseRequest
    {
        [Required]
        public int RideId { get; set; }
    }
}
