using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcabGo.Core.Data.Ride.Filters
{
    public class RideFilter : BaseRequest
    {
        public bool OnlyAvailable { get; set; }
    }
}
