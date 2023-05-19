using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Entities
{
    public abstract class BaseEntity
    {
        [KeyAttribute]
        public int Id { get; set; }
    }
}
