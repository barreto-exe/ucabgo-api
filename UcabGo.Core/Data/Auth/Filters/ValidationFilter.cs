using Newtonsoft.Json;

namespace UcabGo.Core.Data.Auth.Filters
{
    public class ValidationFilter : BaseRequest
    {
        public string ValidationEmail { get; set; }
        public string ValidationGuid { get; set; }
        [JsonIgnore]
        public string ValidationUrl { get; set; }
    }
}
