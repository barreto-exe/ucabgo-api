using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Soscontact.Inputs
{
    public class SoscontactUpdateInput : BaseRequest
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
    }
}
