using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Passenger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IRideService rideService;
        private readonly ILocationService locationService;
        private readonly IMapper mapper;
        public PassengerService(IUnitOfWork unitOfWork, IUserService userService, IRideService rideService, ILocationService locationService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.rideService = rideService;
            this.locationService = locationService;
            this.mapper = mapper;
        }

        public async Task<Passenger> GetById(int id)
        {
            return await unitOfWork.PassengerRepository.GetById(id);
        }
        public async Task<PassengerDto> AskForRide(PassengerInput input)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(input.Email)).Id;

            //Validate if ride exists and its available
            var ride = await rideService.GetById(input.Ride);
            bool isAvailable = Convert.ToBoolean(ride?.IsAvailable);
            if (ride == null || !isAvailable)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            //Validate if final location exists
            var finalLocation = await locationService.GetById(input.FinalLocation);
            if (finalLocation == null || finalLocation.User != idUser)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }

            //Validate if user is already in a ride
            var rideDto = (await GetRides(new RideFilter()
            {
                Email = input.Email,
                OnlyAvailable = true
            })).FirstOrDefault();
            var passengerDto = rideDto?.Passengers?.FirstOrDefault(p => p.Id == idUser);
            var passengerIsInRide =
                passengerDto?.TimeAccepted != null &&
                passengerDto?.TimeCancelled == null &&
                passengerDto?.TimeIgnored == null &&
                passengerDto?.TimeFinished == null;
            if (rideDto != null && passengerIsInRide)
            {
                throw new Exception("ALREADY_IN_RIDE");
            }

            //Validate if available seats
            var activePassengers = ride.Passengers.Where(p =>
                p.TimeAccepted != null &&
                p.TimeCancelled == null &&
                p.TimeIgnored == null &&
                p.TimeFinished == null);
            int availableSeats = ride.SeatQuantity - activePassengers.Count();
            if (availableSeats <= 0)
            {
                throw new Exception("NO_AVAILABLE_SEATS");
            }

            //Create passenger
            var item = mapper.Map<Passenger>(input);
            item.User = idUser;
            item.TimeSolicited = DateTime.Now;

            await unitOfWork.PassengerRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<PassengerDto>(item);
        }
        public async Task<IEnumerable<RideDto>> GetRides(RideFilter filter)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(filter.Email)).Id;

            //Get ride Ids from user
            var ridesIds = unitOfWork
                .PassengerRepository
                .GetAll()
                .Where(p => p.User == idUser)
                .Select(p => p.Ride)
                .ToList();

            //Get available rides
            var rides = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    "VehicleNavigation",
                    "FinalLocationNavigation",
                    "DriverNavigation");
            List<Ride> result;
            if (filter.OnlyAvailable)
            {
                result = rides
                    .Where(r => ridesIds.Contains(r.Id) && Convert.ToBoolean(r.IsAvailable))
                    .ToList();
            }
            else
            {
                result = rides
                    .Where(r => ridesIds.Contains(r.Id))
                    .ToList();
            }
            var dtos = mapper.Map<List<RideDto>>(result);

            dtos.ForEach(async dto =>
            {
                dto.Passengers = await rideService.GetPassengers(dto.Id);
            });

            return dtos;
        }
        public async Task<PassengerDto> CancelRide(CancelRideInput input)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(input.Email)).Id;

            //Get ride
            var ride = await rideService.GetById(input.RideId);
            if (ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            //Validate if user is in ride
            var passenger = unitOfWork
                .PassengerRepository
                .GetAll()
                .Where(p => p.User == idUser && p.Ride == input.RideId)
                .OrderByDescending(p => p.TimeSolicited)
                .FirstOrDefault();
            if (passenger == null)
            {
                throw new Exception("NOT_IN_RIDE");
            }

            passenger.TimeCancelled = DateTime.Now;
            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<PassengerDto>(passenger);
        }
        public async Task<PassengerDto> FinishRide(FinishRideInput input)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(input.Email)).Id;

            //Validate if user is in ride
            var passenger = unitOfWork
                .PassengerRepository
                .GetAll()
                .Where(p => p.User == idUser && p.Ride == input.RideId)
                .OrderByDescending(p => p.TimeSolicited)
                .FirstOrDefault();
            if (passenger == null)
            {
                throw new Exception("NOT_IN_RIDE");
            }

            //Get ride
            var ride = await rideService.GetById(input.RideId);
            if (ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            //Cant finish passenger if Ride is not started
            if (ride.TimeStarted == null || passenger.TimeAccepted == null)
            {
                throw new Exception("RIDE_NOT_STARTED");
            }


            passenger.TimeFinished = DateTime.Now;
            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<PassengerDto>(passenger);
        }
    }
}
