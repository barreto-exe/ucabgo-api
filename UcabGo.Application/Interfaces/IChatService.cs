using UcabGo.Core.Data.Chat.Dtos;
using UcabGo.Core.Data.Chat.Input;

namespace UcabGo.Application.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatmessageDto>> GetAllMessages(string userEmail, int rideId);
        Task<ChatmessageDto> SendMessage(ChatmessageInput input);
    }
}
