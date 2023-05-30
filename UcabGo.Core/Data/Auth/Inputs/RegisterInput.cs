using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Auth.Inputs
{
    public class RegisterInput : BaseRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must: (1) Be at least 8 characters long. (2) Contain at least one capital letter. (3) Contain at least one number. (4) Contain at least one special character.")]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string? SecondName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? SecondLastName { get; set; }
    }
}
