using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Ride.Inputs
{
    public class RideAvailableInput : BaseRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
