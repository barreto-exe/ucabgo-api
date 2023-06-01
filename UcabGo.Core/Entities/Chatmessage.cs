namespace UcabGo.Core.Entities
{
    public partial class Chatmessage : BaseEntity
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int User { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? TimeSent { get; set; }

        public virtual Ride RideNavigation { get; set; } = null!;
        public virtual User UserNavigation { get; set; } = null!;
    }
}
