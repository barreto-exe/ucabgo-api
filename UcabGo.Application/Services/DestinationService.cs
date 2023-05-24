using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Destination.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public DestinationService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<DestinationDto>> GetAllDtos(string userEmail)
        {
            var items = await GetAll(userEmail);
            var itemDtos = mapper.Map<IEnumerable<DestinationDto>>(items);
            return itemDtos;
        }
        public async Task<IEnumerable<Destination>> GetAll(string userEmail)
        {
            var list = unitOfWork.DestinationRepository.GetAllIncluding(x => x.UserNavigation);

            var result =
                from item in list
                where item.UserNavigation.Email == userEmail
                select item;
            return result.ToList();
        }
        public async Task<Destination> GetById(int id)
        {
            return await unitOfWork.DestinationRepository.GetById(id);
        }
        public async Task<DestinationDto> Create(DestinationInput input, bool isRegistering = false)
        {
            var item = mapper.Map<Destination>(input);

            if (IsUCABGuayana(item) && !isRegistering)
            {
                throw new Exception("UCAB_DESTINATION_ALREADY_CREATED");
            }

            int idUser = (await userService.GetByEmail(input.Email)).Id;
            item.User = idUser;
            await unitOfWork.DestinationRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<DestinationDto>(itemDb);
            dto.Driver = mapper.Map<UserDto>(item.UserNavigation);
            return dto;

            static bool IsUCABGuayana(Destination input)
            {
                if (input.Alias == "UCAB Guayana") return true;
                if (input.Zone == "UCAB Guayana") return true;

                // TODO - Implement more validations

                // ...

                return false;
            }
        }
        public async Task<DestinationDto> Update(DestinationUpdateInput input)
        {
            if (input.Alias == "UCAB Guayana" || input.Zone == "UCAB Guayana")
            {
                throw new Exception("UCAB_DESTINATION_IS_READONLY");
            }

            var itemDb = await GetById(input.Id);
            if (itemDb == null)
            {
                throw new Exception("DESTINATION_NOT_FOUND");
            }

            itemDb.Alias = input.Alias ?? itemDb.Alias;
            itemDb.Zone = input.Zone ?? itemDb.Zone;
            itemDb.Detail = input.Detail ?? itemDb.Detail;
            itemDb.Latitude = input.Latitude ?? itemDb.Latitude;
            itemDb.Longitude = input.Longitude ?? itemDb.Longitude;
            itemDb.IsActive = input.IsActive != null ? Convert.ToUInt64(input.IsActive) : itemDb.IsActive;

            unitOfWork.DestinationRepository.Update(itemDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<DestinationDto>(itemDb);
            return dto;
        }
        public async Task<DestinationDto> Delete(string userEmail, int id)
        {
            var items = await GetAll(userEmail);
            var itemDb = items.FirstOrDefault(x => x.Id == id);

            if (itemDb == null)
            {
                throw new Exception("DESTINATION_NOT_FOUND");
            }
            if (itemDb.Alias == "UCAB Guayana" || itemDb.Zone == "UCAB Guayana")
            {
                throw new Exception("UCAB_DESTINATION_IS_READONLY");
            }

            await unitOfWork.DestinationRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<DestinationDto>(itemDb);
            return dto;
        }
    }
}
