using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Chat.Dtos;
using UcabGo.Core.Data.Chat.Input;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public ChatService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ChatmessageDto>> GetAllMessages(string userEmail, int rideId)
        {
            //Validate if user is passenger or driver of the ride
            var user = await userService.GetByEmail(userEmail);
            var ride = unitOfWork.RideRepository.GetAllIncluding("Passengers").FirstOrDefault(x => x.Id == rideId);

            bool isDriver = ride?.Driver == user.Id;
            bool isPassenger = ride?.Passengers.FirstOrDefault(p => p.User == user.Id) != null;

            if (!isDriver && !isPassenger)
            {
                throw new Exception("CHAT_NOT_FOUND");
            }

            var messages = unitOfWork.ChatmessageRepository.GetAll();
            var filteredMessages = from m in messages
                                   where m.Ride == rideId
                                   select m;

            return mapper.Map<IEnumerable<ChatmessageDto>>(filteredMessages.ToList());
        }

        public async Task<ChatmessageDto> SendMessage(ChatmessageInput input)
        {
            //Validate if user is passenger or driver of the ride
            var user = await userService.GetByEmail(input.Email);
            var ride = unitOfWork.RideRepository.GetAllIncluding("Passengers").FirstOrDefault(x => x.Id == input.Ride);

            bool isDriver = ride?.Driver == user.Id;
            bool isPassenger = ride?.Passengers.FirstOrDefault(p => p.User == user.Id) != null;

            if (!isDriver && !isPassenger)
            {
                throw new Exception("CHAT_NOT_FOUND");
            }

            var message = new Chatmessage
            {
                Ride = input.Ride,
                User = user.Id,
                Content = input.Content,
                TimeSent = DateTime.Now
            };

            await unitOfWork.ChatmessageRepository.Add(message);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ChatmessageDto>(message);
        }
    }
}
