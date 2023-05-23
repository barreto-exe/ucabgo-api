using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Destination.Inputs
{
    public class DestinationInput : BaseRequest
    {
        [Required]
        public string Alias { get; set; }
        [Required]
        public string Zone { get; set; }
        public string? Detail { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
    }
}
