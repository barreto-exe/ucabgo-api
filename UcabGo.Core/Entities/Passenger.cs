namespace UcabGo.Core.Entities
{
    public partial class Passenger : BaseEntity
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int InitialLocation { get; set; }
        public int User { get; set; }
        public DateTime TimeSolicited { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public DateTime? TimeIgnored { get; set; }
        public DateTime? TimeCancelled { get; set; }

        public virtual Location InitialLocationNavigation { get; set; } = null!;
        public virtual Ride RideNavigation { get; set; } = null!;
        public virtual User UserNavigation { get; set; } = null!;
    }
}
