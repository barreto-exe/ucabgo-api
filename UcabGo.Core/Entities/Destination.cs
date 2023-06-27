namespace UcabGo.Core.Entities
{
    public partial class Destination : BaseEntity
    {
        public Destination()
        {
            Rides = new HashSet<Ride>();
        }

        public int Id { get; set; }
        public int User { get; set; }
        public string Alias { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string? Detail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public ulong IsDeleted { get; set; }

        public virtual User UserNavigation { get; set; } = null!;
        public virtual ICollection<Ride> Rides { get; set; }
    }
}
