using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.User.Inputs
{
    public class PhoneInput : BaseRequest
    {
        [Required]
        public string Phone { get; set; }
    }
}
