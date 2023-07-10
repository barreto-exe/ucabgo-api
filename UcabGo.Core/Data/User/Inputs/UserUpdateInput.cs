using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.User.Inputs
{
    public class UserUpdateInput : BaseRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^(?:(?:\\+58)(?:-)?(?:4(?:14|24|12||26))|(?:0(?:414|424|412|416|426)))[-]?[0-9]{7}$", ErrorMessage = "The phone has an invalid format.")]
        public string Phone { get; set; }
    }
}
