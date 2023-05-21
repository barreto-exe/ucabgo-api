using System;
using System.Collections.Generic;

namespace UcabGo.Core.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            Destinations = new HashSet<Destination>();
            Soscontacts = new HashSet<Soscontact>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SecondName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<Destination> Destinations { get; set; }
        public virtual ICollection<Soscontact> Soscontacts { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
