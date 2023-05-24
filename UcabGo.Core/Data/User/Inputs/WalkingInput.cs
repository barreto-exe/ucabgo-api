using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.User.Inputs
{
    public class WalkingInput : BaseRequest
    {
        [Required]
        public float WalkingDistance { get; set; }
    }
}
