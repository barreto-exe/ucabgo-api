﻿namespace UcabGo.Core.Entities
{
    public partial class Ride : BaseEntity
    {
        public Ride()
        {
            Chatmessages = new HashSet<Chatmessage>();
            Evaluations = new HashSet<Evaluation>();
            Passengers = new HashSet<Passenger>();
        }

        public int Id { get; set; }
        public int Driver { get; set; }
        public int Vehicle { get; set; }
        public int FinalLocation { get; set; }
        public float LatitudeOrigin { get; set; }
        public float LongitudeOrigin { get; set; }
        public int SeatQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime? TimeEnded { get; set; }
        public DateTime? TimeStarted { get; set; }
        public DateTime? TimeCanceled { get; set; }

        public virtual User DriverNavigation { get; set; } = null!;
        public virtual Location FinalLocationNavigation { get; set; } = null!;
        public virtual Vehicle VehicleNavigation { get; set; } = null!;
        public virtual ICollection<Chatmessage> Chatmessages { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Passenger> Passengers { get; set; }
    }
}
