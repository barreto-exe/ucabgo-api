using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Inputs;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class RideService : IRideService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IVehicleService vehicleService;
        private readonly IDestinationService destinationService;
        private readonly IMapper mapper;
        public RideService(IUnitOfWork unitOfWork, IUserService userService, IVehicleService vehicleService, IDestinationService destinationService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.vehicleService = vehicleService;
            this.destinationService = destinationService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<RideDto>> GetAll(string driverEmail)
        {
            var items = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.DestinationNavigation,
                    r => r.DriverNavigation);

            var ridesFromDriver = from r in items
                                  where r.DriverNavigation.Email == driverEmail
                                  select r;

            var ridesDtos = ridesFromDriver.Select(x => new RideDto
            {
                Id = x.Id,
                Driver = mapper.Map<UserDto>(x.DriverNavigation),
                Vehicle = mapper.Map<VehicleDto>(x.VehicleNavigation),
                Destination = mapper.Map<DestinationDto>(x.DestinationNavigation),
                SeatQuantity = x.SeatQuantity,
                LongitudeOrigin = x.LongitudeOrigin,
                LatitudeOrigin = x.LatitudeOrigin,
                IsAvailable = Convert.ToBoolean(x.IsAvailable),
                Passengers = x.Passengers,
            });

            return ridesDtos;
        }
        public async Task<IEnumerable<Ride>> GetAll()
        {
            var result = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.Vehicle, 
                    r => r.Destination, 
                    r => r.Driver);

            return result.ToList();
        }
        public async Task<Ride> GetById(int id)
        {
            return await unitOfWork.RideRepository.GetById(id);
        }
        public async Task<RideDto> Create(RideInput input)
        {
            //Validate if has an active ride
            var rides = await GetAll(input.Email);
            var activeRide = rides.FirstOrDefault(x => x.IsAvailable);
            if (activeRide != null)
            {
                throw new Exception("ACTIVE_RIDE_FOUND");
            }

            //Validate if he owns a vehicle with given id
            var vehicles = await vehicleService.GetAll(input.Email);
            var vehicle = vehicles.FirstOrDefault(x => x.Id == input.Vehicle);
            if (vehicle == null)
            {
                throw new Exception("VEHICLE_NOT_FOUND");
            }

            //Validate if he owns a destination with given id
            var destinations = await destinationService.GetAll(input.Email);
            var destination = destinations.FirstOrDefault(x => x.Id == input.Destination);
            if (destination == null)
            {
                throw new Exception("DESTINATION_NOT_FOUND");
            }

            var item = mapper.Map<Ride>(input);

            //Assign user id and available true
            int idUser = (await userService.GetByEmail(input.Email)).Id;
            item.Driver = idUser;
            item.IsAvailable = Convert.ToUInt64(true);
            
            //Save changes
            await unitOfWork.RideRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            //Map dto and return
            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<RideDto>(itemDb);
            dto.Driver = mapper.Map<UserDto>(itemDb.DriverNavigation);
            dto.Vehicle = mapper.Map<VehicleDto>(itemDb.VehicleNavigation);
            dto.Destination = mapper.Map<DestinationDto>(itemDb.DestinationNavigation);
            return dto;
        }
        public async Task<RideDto> Update(RideUpdateInput input)
        {
            var itemDb = await GetById(input.Id);
            if (itemDb == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            itemDb.IsAvailable = Convert.ToUInt64(input.IsAvailable);

            unitOfWork.RideRepository.Update(itemDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(itemDb);
            return dto;
        }
        public async Task<RideDto> Delete(string driverEmail, int id)
        {
            var items = await GetAll(driverEmail);
            var itemDb = items.FirstOrDefault(x => x.Id == id);
            if (itemDb == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            await unitOfWork.RideRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(itemDb);
            return dto;
        }
    }
}
