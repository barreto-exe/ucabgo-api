using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcabGo.Core.Data.Destinations.Inputs
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
