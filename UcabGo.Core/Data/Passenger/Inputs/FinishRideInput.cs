using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Passenger.Inputs
{
    public class FinishRideInput : BaseRequest
    {
        [Required]
        public int RideId { get; set; }
    }
}
