using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Ride.Inputs
{
    public class RideInput : BaseRequest
    {
        [Required]
        public int Vehicle { get; set; }
        [Required]
        public int Destination { get; set; }
        [Required]
        public int SeatQuantity { get; set; }
        [Required]
        public float LatitudeOrigin { get; set; }
        [Required]
        public float LongitudeOrigin { get; set; }
    }
}
