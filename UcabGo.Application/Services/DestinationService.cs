using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Destinations.Dtos;
using UcabGo.Core.Data.Destinations.Inputs;
using UcabGo.Core.Data.Soscontact.Dto;
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
            var itemsDtos = items.Select(x => new DestinationDto
            {
                Id = x.Id,
                Driver = mapper.Map<UserDto>(x.UserNavigation),
                Alias = x.Alias,
                Zone = x.Zone,
                Detail = x.Detail,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
            });

            return itemsDtos;
        }
        public async Task<IEnumerable<Destination>> GetAll(string userEmail)
        {
            var list = unitOfWork.DestinationRepository.GetAllIncluding(x => x.UserNavigation);
            var users = unitOfWork.UserRepository.GetAll();

            var result =
                from item in list
                join u in users on item.User equals u.Id
                where item.UserNavigation.Email == userEmail
                select item;

            return result.ToList();
        }
        public async Task<Destination> GetById(int id)
        {
            return await unitOfWork.DestinationRepository.GetById(id);
        }
        public async Task<DestinationDto> Create(DestinationInput input)
        {
            var item = mapper.Map<Destination>(input);

            int idUser = (await userService.GetByEmail(input.Email)).Id;
            item.User = idUser;
            await unitOfWork.DestinationRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<DestinationDto>(itemDb);
            return dto;
        }
        public async Task<DestinationDto> Update(DestinationUpdateInput input)
        {
            var itemDb = await GetById(input.Id);

            if (itemDb == null)
            {
                throw new Exception("DESTINATION_NOT_FOUND");
            }

            itemDb.Alias = input.Alias;
            itemDb.Zone = input.Zone;
            itemDb.Detail = input.Detail;
            itemDb.Latitude = input.Latitude;
            itemDb.Longitude = input.Longitude;

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

            await unitOfWork.DestinationRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<DestinationDto>(itemDb);
            return dto;
        }
    }
}
