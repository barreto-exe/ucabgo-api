using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;

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
