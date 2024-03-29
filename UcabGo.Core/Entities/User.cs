﻿namespace UcabGo.Core.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            Chatmessages = new HashSet<Chatmessage>();
            EvaluationEvaluatedNavigations = new HashSet<Evaluation>();
            EvaluationEvaluatorNavigations = new HashSet<Evaluation>();
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
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public double? WalkingDistance { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsValidated { get; set; }
        public string? ValidationGuid { get; set; }

        public virtual ICollection<Chatmessage> Chatmessages { get; set; }
        public virtual ICollection<Evaluation> EvaluationEvaluatedNavigations { get; set; }
        public virtual ICollection<Evaluation> EvaluationEvaluatorNavigations { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Passenger> Passengers { get; set; }
        public virtual ICollection<Ride> Rides { get; set; }
        public virtual ICollection<Soscontact> Soscontacts { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
