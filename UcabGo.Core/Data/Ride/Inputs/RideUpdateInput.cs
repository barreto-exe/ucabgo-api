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
    public class RideUpdateInput : BaseRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
    }
}
