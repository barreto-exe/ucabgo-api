using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IVehicleService vehicleService;
        private readonly IRideService rideService;
        private readonly ILocationService locationService;
        private readonly IMapper mapper;
        public DriverService(IUnitOfWork unitOfWork, IUserService userService, IVehicleService vehicleService, IRideService rideService, ILocationService locationService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.vehicleService = vehicleService;
            this.rideService = rideService;
            this.locationService = locationService;
            this.mapper = mapper;
        }

        public async Task<RideDto> CreateRide(RideInput input)
        {
            //Validate if has an active ride
            var rides = await rideService.GetAll(input.Email);
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

            bool reachLimit = input.SeatQuantity < 1 || input.SeatQuantity > 5;
            if (reachLimit)
            {
                throw new Exception("SEAT_LIMIT_REACHED");
            }

            //Validate if he owns a destination with given id
            var destinations = await locationService.GetAll(input.Email);
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
            var itemDb = await rideService.GetById(item.Id);
            var dto = mapper.Map<RideDto>(itemDb);
            dto.Driver = mapper.Map<UserDto>(itemDb.DriverNavigation);
            dto.Vehicle = mapper.Map<VehicleDto>(itemDb.VehicleNavigation);
            dto.Destination = mapper.Map<LocationDto>(itemDb.FinalLocationNavigation);
            return dto;
        }
        public async Task<RideDto> StartRide(RideAvailableInput input)
        {
            var rides = await rideService.GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await rideService.GetById(input.Id);

            //Validate if ride is not available
            bool cantStartRide =
                rideDb.TimeStarted != null || //Can't start if already started
                rideDb.TimeEnded != null || //Can't start if already ended
                rideDb.TimeCanceled != null || //Can't start if already canceled
                !rideDb.Passengers.Any() || //Can't start if 0 passengers
                !Convert.ToBoolean(rideDb.IsAvailable);
            if (cantStartRide)
            {
                throw new Exception("CANT_START_RIDE");
            }
            
            rideDb.TimeStarted = DateTime.Now;
            rideDb.IsAvailable = Convert.ToUInt64(false);

            //Autoignore all passengers that were not accepted when ride started.
            var passengers = rideDb.Passengers.Where(x => x.TimeAccepted == null);
            foreach (var passenger in passengers)
            {
                passenger.TimeIgnored = rideDb.TimeStarted;
            }

            unitOfWork.RideRepository.Update(rideDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(rideDb);
            return dto;
        }
        public async Task<RideDto> CompleteRide(RideAvailableInput input)
        {
            var rides = await rideService.GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await rideService.GetById(input.Id);

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
            var rides = await rideService.GetAll(input.Email);
            var rideDto = rides.FirstOrDefault(x => x.Id == input.Id);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            var rideDb = await rideService.GetById(input.Id);

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

            //Autocancel all passengers if ride was canceled.
            foreach (var passenger in rideDb.Passengers)
            {
                passenger.TimeCancelled = DateTime.Now;
                unitOfWork.PassengerRepository.Update(passenger);
            }

            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<RideDto>(rideDb);
            return dto;
        }

        public async Task<PassengerDto> AcceptPassenger(string driverEmail, int rideId, int passengerId)
        {
            var rides = await rideService.GetAll(driverEmail);
            var rideDto = rides.FirstOrDefault(x => x.Id == rideId);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passenger = await unitOfWork.PassengerRepository.GetById(passengerId);
            if (passenger == null)
            {
                throw new Exception("PASSENGER_NOT_FOUND");
            }
            bool canAccept = passenger.TimeAccepted == null && passenger.TimeIgnored == null && passenger.TimeCancelled == null;
            if (!canAccept)
            {
                throw new Exception("REQUEST_NOT_AVAILABLE_OR_ACCEPTED");
            }

            passenger.TimeAccepted = DateTime.Now;

            var userId = passenger.User;
            var locationId = passenger.FinalLocation;

            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);

            //PassengerDto mapping not working correctly. User and Location assigned manually
            //TODO - Fix mapping for PassengerDto
            dto.User = mapper.Map<UserDto>(await userService.GetById(userId));
            dto.FinalLocation = mapper.Map<LocationDto>(await locationService.GetById(locationId));
            return dto;
        }
        public async Task<PassengerDto> IgnorePassenger(string driverEmail, int rideId, int passengerId)
        {
            var rides = await rideService.GetAll(driverEmail);
            var rideDto = rides.FirstOrDefault(x => x.Id == rideId);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passenger = await unitOfWork.PassengerRepository.GetById(passengerId);
            if (passenger == null)
            {
                throw new Exception("PASSENGER_NOT_FOUND");
            }

            bool canIgnore =
                passenger.TimeAccepted == null &&
                passenger.TimeIgnored == null &&
                passenger.TimeCancelled == null;
            if (!canIgnore)
            {
                throw new Exception("REQUEST_NOT_AVAILABLE_OR_ACCEPTED");
            }

            passenger.TimeIgnored = DateTime.Now;

            var userId = passenger.User;
            var locationId = passenger.FinalLocation;

            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);

            //PassengerDto mapping not working correctly. User and Location assigned manually
            dto.User = mapper.Map<UserDto>(await userService.GetById(userId));
            dto.FinalLocation = mapper.Map<LocationDto>(await locationService.GetById(locationId));
            return dto;
        }
        public async Task<PassengerDto> CancelPassenger(string driverEmail, int rideId, int passengerId)
        {
            var rides = await rideService.GetAll(driverEmail);
            var rideDto = rides.FirstOrDefault(x => x.Id == rideId);
            if (rideDto == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passenger = await unitOfWork.PassengerRepository.GetById(passengerId);
            if (passenger == null)
            {
                throw new Exception("PASSENGER_NOT_FOUND");
            }

            //Can cancel if not cancelled and ride is not cancelled and not ended
            bool canCancel =
                passenger.TimeAccepted != null &&
                passenger.TimeCancelled == null &&
                passenger.TimeIgnored == null &&
                passenger.TimeFinished == null &&
                rideDto.TimeCanceled == null &&
                rideDto.TimeEnded == null;
            if (!canCancel)
            {
                throw new Exception("REQUEST_ALREADY_CANCELED_OR_NOT_ACCEPTED");
            }

            passenger.TimeCancelled = DateTime.Now;

            var userId = passenger.User;
            var locationId = passenger.FinalLocation;

            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);

            //PassengerDto mapping not working correctly. User and Location assigned manually
            dto.User = mapper.Map<UserDto>(await userService.GetById(userId));
            dto.FinalLocation = mapper.Map<LocationDto>(await locationService.GetById(locationId));
            return dto;
        }
    }
}
