using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Auth.Inputs
{
    public class RegisterInput : BaseRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter and one number.")]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string? SecondName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? SecondLastName { get; set; }
    }
}
