using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Passanger.Inputs
{
    public class PassengerInput : BaseRequest
    {
        [Required]
        public int Ride { get; set; }
        [Required]
        public int FinalLocation { get; set; }
        [Required]
        public float LatitudeOrigin { get; set; }
        [Required]
        public float LongitudeOrigin { get; set; }
    }
}
