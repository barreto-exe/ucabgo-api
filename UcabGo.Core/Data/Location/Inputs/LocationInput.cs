using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Core.Data.User.Dto;

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
        [Required]
        public bool IsHome { get; set; }
    }
}
