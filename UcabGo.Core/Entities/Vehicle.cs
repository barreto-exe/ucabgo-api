using System;
using System.Collections.Generic;

namespace UcabGo.Core.Entities
{
    public partial class Vehicle : BaseEntity
    {
        public int Id { get; set; }
        public int User { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Plate { get; set; } = null!;
        public string Color { get; set; } = null!;

        public virtual User UserNavigation { get; set; } = null!;
    }
}
