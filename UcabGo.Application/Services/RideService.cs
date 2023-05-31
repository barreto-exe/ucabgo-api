using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.Ride.Inputs;
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

        public async Task<IEnumerable<RideDto>> GetMathchingAll(MatchingFilter filter)
        {
            var items = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    "VehicleNavigation",
                    "DestinationNavigation",
                    "DriverNavigation",
                    "Passengers.UserNavigation",
                    "Passengers.InitialLocationNavigation");

            var rides = from r in items
                        where
                            r.IsAvailable == Convert.ToUInt64(true) &&
                            r.DriverNavigation.Email != filter.Email
                        select r;

            //TODO - Matching algorithm here

            var ridesDtos = mapper.Map<IEnumerable<RideDto>>(rides.ToList());
            return ridesDtos;
        }

        public async Task<IEnumerable<RideDto>> GetAll(string driverEmail, bool onlyAvailable = false)
        {
            var items = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.DestinationNavigation,
                    r => r.DriverNavigation);

            IQueryable<Ride>? ridesFromDriver;
            if (onlyAvailable)
            {
                ridesFromDriver = from r in items
                                  where
                                    r.DriverNavigation.Email == driverEmail &&
                                    (r.IsAvailable == Convert.ToUInt64(true) ||
                                    (r.TimeStarted != null && r.TimeEnded == null && r.TimeCanceled == null))
                                  select r;
            }
            else
            {
                ridesFromDriver = from r in items
                                  where r.DriverNavigation.Email == driverEmail
                                  select r;
            }


            var ridesDtos = mapper.Map<IEnumerable<RideDto>>(ridesFromDriver.ToList());
            return ridesDtos;
        }
        public async Task<IEnumerable<Ride>> GetAll()
        {
            var result = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.DestinationNavigation,
                    r => r.DriverNavigation,
                    r => r.Passengers);

            return result.ToList();
        }
        public async Task<Ride> GetById(int id)
        {
            var result = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    "VehicleNavigation",
                    "DestinationNavigation",
                    "DriverNavigation",
                    "Passengers.UserNavigation",
                    "Passengers.InitialLocationNavigation");

            var ride = result.FirstOrDefault(x => x.Id == id);
            return ride;
        }
        public async Task<RideDto> Create(RideInput input)
        {
            //Validate if has an active ride
            var rides = await GetAll(input.Email);
            var activeRide = rides.FirstOrDefault(
                 x => x.IsAvailable ||
                (x.TimeStarted != null && x.TimeCanceled == null && x.TimeEnded == null));

            //If is available, or has started and not been canceled nor ended, then is consider a ride in progress.
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
            item.TimeCreated = DateTime.Now;

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
        public async Task<RideDto> StartRide(RideAvailableInput input)
        {
            var rides = await GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await GetById(input.Id);

            //Validate if ride is not available
            bool cantStartRide =
                rideDb.TimeStarted != null || //Can't start if already started
                rideDb.TimeEnded != null || //Can't start if already ended
                rideDb.TimeCanceled != null || //Can't start if already canceled
                !Convert.ToBoolean(rideDb.IsAvailable);
            if (cantStartRide)
            {
                throw new Exception("CANT_START_RIDE");
            }

            rideDb.TimeStarted = DateTime.Now;
            rideDb.IsAvailable = Convert.ToUInt64(false);

            unitOfWork.RideRepository.Update(rideDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(rideDb);
            return dto;
        }
        public async Task<RideDto> CompleteRide(RideAvailableInput input)
        {
            var rides = await GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await GetById(input.Id);

            //Validate if ride is not available
            bool cantCompleteRide =
                rideDb.TimeStarted == null || //Can't complete if not started
                rideDb.TimeEnded != null || //Can't complete if already ended
                rideDb.TimeCanceled != null || //Can't complete if already canceled
                Convert.ToBoolean(rideDb.IsAvailable); //Can't complete if available
            if (cantCompleteRide)
            {
                throw new Exception("CANT_COMPLETE_RIDE");
            }

            rideDb.TimeEnded = DateTime.Now;
            rideDb.IsAvailable = Convert.ToUInt64(false);

            unitOfWork.RideRepository.Update(rideDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(rideDb);
            return dto;
        }
        public async Task<RideDto> CancelRide(RideAvailableInput input)
        {
            var rides = await GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await GetById(input.Id);

            //Validate if ride is not available
            bool cantCancelRide =
                rideDb.TimeStarted != null || //Can't cancel if already started
                rideDb.TimeEnded != null || //Can't cancel if already ended
                rideDb.TimeCanceled != null || //Can't cancel if already canceled
                !Convert.ToBoolean(rideDb.IsAvailable);
            if (cantCancelRide)
            {
                throw new Exception("CANT_CANCEL_RIDE");
            }

            rideDb.TimeCanceled = DateTime.Now;
            rideDb.IsAvailable = Convert.ToUInt64(false);

            unitOfWork.RideRepository.Update(rideDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(rideDb);
            return dto;
        }
    }
}
