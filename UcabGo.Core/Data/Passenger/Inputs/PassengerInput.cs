using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Passanger.Inputs
{
    public class PassengerInput : BaseRequest
    {
        [Required]
        public int Ride { get; set; }
        [Required]
        public int InitialLocation { get; set; }
    }
}
