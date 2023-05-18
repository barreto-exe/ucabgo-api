﻿namespace UcabGo.Core.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            Soscontacts = new HashSet<Soscontact>();
            Vehicles = new HashSet<Vehicle>();
        }

        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SecondName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<Soscontact> Soscontacts { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
