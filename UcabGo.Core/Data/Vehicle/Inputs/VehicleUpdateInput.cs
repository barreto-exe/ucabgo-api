using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Vehicle.Inputs
{
    public class VehicleUpdateInput : BaseRequest
    {
        [Required]
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Plate { get; set; }
        public string? Color { get; set; }
    }
}
