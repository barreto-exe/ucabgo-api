using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace UcabGo.Core.Data.Chat.Input
{
    public class ChatmessageInput : BaseRequest
    {
        [Required]
        [JsonIgnore]
        public int Ride { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
