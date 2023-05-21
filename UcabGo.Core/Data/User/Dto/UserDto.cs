namespace UcabGo.Core.Data.User.Dto
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? SecondName { get; set; }

        public string LastName { get; set; } = null!;

        public string? SecondLastName { get; set; }

        public string? Phone { get; set; }
    }
}
