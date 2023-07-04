namespace UcabGo.Core.Entities
{
    public partial class Passenger : BaseEntity
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int User { get; set; }
        public int FinalLocation { get; set; }
        public float LatitudeOrigin { get; set; }
        public float LongitudeOrigin { get; set; }
        public DateTime TimeSolicited { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public DateTime? TimeIgnored { get; set; }
        public DateTime? TimeCancelled { get; set; }
        public DateTime? TimeFinished { get; set; }
        public bool IsActive { get => TimeIgnored == null && TimeCancelled == null && TimeFinished == null; }

        public virtual Location FinalLocationNavigation { get; set; } = null!;
        public virtual Ride RideNavigation { get; set; } = null!;
        public virtual User UserNavigation { get; set; } = null!;
    }
}
