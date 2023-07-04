using AutoMapper;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<LocationDto> GetHome(string userEmail)
        {
            var items = await GetAll(userEmail);
            var item = items.FirstOrDefault(x => Convert.ToBoolean(x.IsHome));
            var itemDto = mapper.Map<LocationDto>(item);
            return itemDto;
        }
        public async Task<IEnumerable<LocationDto>> GetDefaultLocations(string userEmail)
        {
            var items = await GetAll(userEmail);
            var ucabAndHome = items.Where(x => x.Alias == "UCAB Guayana" || Convert.ToBoolean(x.IsHome));
            var itemsDtos = mapper.Map<IEnumerable<LocationDto>>(ucabAndHome);
            return itemsDtos;
        }
        public async Task<IEnumerable<LocationDto>> GetAllDtos(string userEmail)
        {
            var items = await GetAll(userEmail);
            items = items.Where(x => x.IsDeleted == Convert.ToUInt64(false));
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
                where item.UserNavigation.Email == userEmail &&
                item.IsDeleted == Convert.ToUInt64(false)
                select item;

            return result.ToList();
        }
        public async Task<Location> GetById(int id)
        {
            return await unitOfWork.LocationRepository.GetById(id);
        }
        public async Task<LocationDto> Create(LocationInput input, bool isRegistering = false)
        {
            var item = mapper.Map<Location>(input);

            if (IsUCABGuayana(item) && !isRegistering)
            {
                throw new Exception("UCAB_LOCATION_ALREADY_CREATED");
            }

            if (item.Detail.IsNullOrEmpty())
            {
                throw new Exception("LOCATION_EMPTY_DETAILS");
            }

            if(input.IsHome)
            {
                //Remove current home
                var locations = unitOfWork.LocationRepository.GetAll();
                var currentHome = from l in locations
                                  where l.User == item.User &&
                                  l.IsHome == Convert.ToUInt64(true) &&
                                  l.IsDeleted == Convert.ToUInt64(false)
                                  select l;
                if (currentHome != null)
                {
                    foreach (var itemHome in currentHome)
                    {
                        itemHome.IsHome = Convert.ToUInt64(false);
                        itemHome.IsDeleted = Convert.ToUInt64(true);
                        unitOfWork.LocationRepository.Update(itemHome);
                    }
                }
            }

            //Assign id of user to Location
            int idUser = (await userService.GetByEmail(input.Email)).Id;
            item.User = idUser;

            //Add location to DB
            await unitOfWork.LocationRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<LocationDto>(itemDb);
            return dto;

            static bool IsUCABGuayana(Location input)
            {
                if (input.Alias == "UCAB Guayana") return true;
                if (input.Zone == "UCAB Guayana") return true;

                // TODO - Implement more validations

                // ...

                return false;
            }
        }

        public async Task<LocationDto> Delete(int id, string userEmail)
        {
            var items = await GetAll(userEmail);
            var itemDb = items.FirstOrDefault(x => x.Id == id);
            if (itemDb == null)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }
            if (itemDb.Alias == "UCAB Guayana")
            {
                throw new Exception("UCAB_LOCATION_IS_READONLY");
            }
            if (Convert.ToBoolean(itemDb.IsHome))
            {
                throw new Exception("HOME_LOCATION_IS_READONLY");
            }

            itemDb.IsDeleted = Convert.ToUInt64(true);
            unitOfWork.LocationRepository.Update(itemDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<LocationDto>(itemDb);
            return dto;
        }

    }
}
