using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.User.Inputs
{
    public class ProfilePictureInput : BaseRequest
    {
        [Required]
        public IFormFile Picture { get; set; }
    }
}
