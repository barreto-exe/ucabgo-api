using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public LocationService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllDtos(string userEmail)
        {
            var items = await GetAll(userEmail);
            var itemsDtos = mapper.Map<IEnumerable<LocationDto>>(items);
            return itemsDtos;
        }
        public async Task<IEnumerable<Location>> GetAll(string userEmail)
        {
            var list = unitOfWork.LocationRepository.GetAllIncluding(x => x.UserNavigation);
            var users = unitOfWork.UserRepository.GetAll();

            var result =
                from item in list
                join u in users on item.User equals u.Id
                where item.UserNavigation.Email == userEmail
                select item;

            return result.ToList();
        }
        public async Task<Location> GetById(int id)
        {
            return await unitOfWork.LocationRepository.GetById(id);
        }
        public async Task<LocationDto> Create(LocationInput location)
        {
            var item = mapper.Map<Location>(location);

            int idUser = (await userService.GetByEmail(location.Email)).Id;
            item.User = idUser;
            await unitOfWork.LocationRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<LocationDto>(itemDb);
            return dto;
        }
        public async Task<LocationDto> Update(LocationUpdateInput location)
        {
            var itemDb = await GetById(location.Id);
            if (itemDb == null)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }

            itemDb.Alias     = location.Alias     ?? itemDb.Alias;
            itemDb.Zone      = location.Zone      ?? itemDb.Zone;
            itemDb.Detail    = location.Detail    ?? itemDb.Detail;
            itemDb.Latitude  = location.Latitude  ?? itemDb.Latitude;
            itemDb.Longitude = location.Longitude ?? itemDb.Longitude;

            unitOfWork.LocationRepository.Update(itemDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<LocationDto>(itemDb);
            return dto;
        }

        public async Task<LocationDto> Delete(int id, string userEmail)
        {
            var items = await GetAll(userEmail);
            var itemDb = items.FirstOrDefault(x => x.Id == id);
            if (itemDb == null)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }

            await unitOfWork.LocationRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<LocationDto>(itemDb);
            return dto;
        }
    }
}
