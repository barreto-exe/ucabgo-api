using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Vehicle.Inputs
{
    public class VehicleUpdateInput : BaseRequest
    {
        [Required]
        public int Id { get; set; }
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
