using Newtonsoft.Json;

namespace UcabGo.Core.Data.Chat.Dtos
{
    public class ChatmessageDto : ISignalRDto
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int User { get; set; }
        public string Content { get; set; }
        public DateTime? TimeSent { get; set; }
        public bool IsMine { get; set; }
        public string UserName { get; set; }

        [JsonIgnore]
        public IEnumerable<string> UsersToMessage { get; set; }
    }
}
