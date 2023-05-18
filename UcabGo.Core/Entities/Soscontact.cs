namespace UcabGo.Core.Entities
{
    public partial class Soscontact : BaseEntity
    {
        public int User { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual User UserNavigation { get; set; } = null!;
    }
}
