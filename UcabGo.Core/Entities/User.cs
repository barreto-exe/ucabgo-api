namespace UcabGo.Core.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            Chatmessages = new HashSet<Chatmessage>();
            Destinations = new HashSet<Destination>();
            Locations = new HashSet<Location>();
            Passengers = new HashSet<Passenger>();
            Rides = new HashSet<Ride>();
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
        public double? WalkingDistance { get; set; }

        public virtual ICollection<Chatmessage> Chatmessages { get; set; }
        public virtual ICollection<Destination> Destinations { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Passenger> Passengers { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
        public virtual ICollection<Soscontact> Soscontacts { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
