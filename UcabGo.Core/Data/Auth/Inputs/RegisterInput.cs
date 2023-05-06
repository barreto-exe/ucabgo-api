using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Auth.Inputs
{
    public class RegisterInput : BaseRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string? SecondName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? SecondLastName { get; set; }
    }
}
