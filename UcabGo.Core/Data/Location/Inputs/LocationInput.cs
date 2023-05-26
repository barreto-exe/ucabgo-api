using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Location.Inputs
{
    public class LocationInput : BaseRequest
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
        [JsonIgnore]
        public bool IsHome { get; set; }
    }
}
