using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.User.Inputs
{
    public class WalkingInput : BaseRequest
    {
        [Required]
        public int WalkingDistance { get; set; }
    }
}
