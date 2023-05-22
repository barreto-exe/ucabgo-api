using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Vehicle.Inputs
{
    public class VehicleInput : BaseRequest
    {
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Plate { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
