namespace UcabGo.Core.Entities
{
    public partial class Destination : BaseEntity
    {
        public int Id { get; set; }
        public int User { get; set; }
        public string Alias { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string? Detail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public ulong IsActive { get; set; }

        public virtual User UserNavigation { get; set; } = null!;
    }
}
