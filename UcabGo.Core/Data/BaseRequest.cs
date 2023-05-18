using System.Text.Json.Serialization;

namespace UcabGo.Core.Data
{
    public class BaseRequest
    {
        [JsonIgnore]
        public string Email { get; set; }
    }
}
