namespace UcabGo.Core.Entities
{
    public partial class Passenger : BaseEntity
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int InitialLocation { get; set; }
        public int Passenger1 { get; set; }
        public ulong IsAccepted { get; set; }
        public DateTime TimeSolicited { get; set; }
        public DateTime? TimeAccepted { get; set; }

        public virtual Location InitialLocationNavigation { get; set; } = null!;
        public virtual User Passenger1Navigation { get; set; } = null!;
        public virtual Ride RideNavigation { get; set; } = null!;
    }
}
