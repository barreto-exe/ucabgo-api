namespace UcabGo.Core.Data.Chat.Dtos
{
    public class ChatmessageDto
    {
        public int Id { get; set; }
        public int Ride { get; set; }
        public int User { get; set; }
        public string Content { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
