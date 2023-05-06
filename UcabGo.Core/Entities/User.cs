using System;
using System.Collections.Generic;

namespace UcabGo.Core.Entities;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? SecondName { get; set; }

    public string LastName { get; set; } = null!;

    public string? SecondLastName { get; set; }
}
