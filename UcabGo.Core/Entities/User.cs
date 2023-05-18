namespace UcabGo.Core.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? SecondName { get; set; }
    public string LastName { get; set; } = null!;
    public string? SecondLastName { get; set; }
    public string? Phone { get; set; }
}
