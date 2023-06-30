using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Ride.Filters
{
    public class MatchingFilter : BaseRequest
    {
        [Required]
        public float InitialLatitude { get; set; }
        [Required]
        public float InitialLongitude { get; set; }
        [Required]
        public float FinalLatitude { get; set; }
        [Required]
        public float FinalLongitude { get; set; }
        [Required]
        public int WalkingDistance { get; set; }
        [Required]
        public bool GoingToCampus { get; set; }
    }
}
