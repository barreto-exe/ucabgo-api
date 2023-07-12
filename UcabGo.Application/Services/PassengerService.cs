using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Passenger.Dtos;
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

            var dto = mapper.Map<PassengerDto>(item);
            dto.UsersToMessage = (await rideService.GetUsers(ride))
                .Where(u => u.Email != input.Email)
                .Select(u => u.Email)
                .ToList();
            return dto;
        }
        public async Task<IEnumerable<RideDto>> GetRides(RideFilter filter)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(filter.Email)).Id;

            //Get ride Ids where user is passenger
            var ridesIds = unitOfWork
                .PassengerRepository
                .GetAll()
                .Where(p => p.User == idUser)
                .Select(p => p.Ride)
                .ToList();

            //Get rides
            var rides = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    "VehicleNavigation",
                    "FinalLocationNavigation",
                    "DriverNavigation",
                    "Passengers",
                    "Passengers.UserNavigation");

            var result = rides
                .Where(r => ridesIds.Contains(r.Id)) //Filter by rides where user is passenger
                .Where(r =>

                    //Filter by available
                    (!filter.OnlyAvailable || r.IsAvailable == filter.OnlyAvailable) ||

                    //Or filter by those where passenger has not arrived and ride is not finished
                    (r.Passengers.Any(p => p.User == idUser && p.TimeAccepted != null && p.TimeIgnored == null && p.TimeCancelled == null && p.TimeFinished == null)) &&
                    r.TimeCanceled == null &&
                    r.TimeEnded == null)
                .ToList();

            //Exclude those where passenger was Ignored, Cancelled or Finished
            if(filter.OnlyAvailable)
            {
                result = result.Where(r =>
                {
                    var passengers = r.Passengers.Where(p => p.User == idUser);
                    return passengers.Any(p =>
                        p.TimeIgnored == null &&
                        p.TimeCancelled == null &&
                        p.TimeFinished == null);
                }).ToList();
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
            
            var dto = mapper.Map<PassengerDto>(passenger);
            dto.UsersToMessage = (await rideService.GetUsers(ride))
                .Where(u => u.Email != input.Email)
                .Select(u => u.Email)
                .ToList();
            return dto;
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

            var dto = mapper.Map<PassengerDto>(passenger);
            dto.UsersToMessage = (await rideService.GetUsers(ride))
                .Where(u => u.Email != input.Email)
                .Select(u => u.Email)
                .ToList();
            return dto;
        }

        public async Task<CooldownDto> GetPassengerCooldownTime(string passengerEmail)
        {
            //Get last ride of passenger
            var rides = await GetRides(new RideFilter
            {
                Email = passengerEmail,
                OnlyAvailable = false,
            });
            var lastRide = rides
                .OrderByDescending(x => x.TimeCreated)
                .ToList()
                .FirstOrDefault();

            //If no rides, return 0
            if (lastRide == null)
            {
                return new CooldownDto
                {
                    IsInCooldown = false,
                    Cooldown = TimeSpan.FromSeconds(0),
                };
            }

            //Get TimeFinished of the passanger with same email
            var timeFinished = lastRide
                .Passengers
                .Where(p => p.User.Email == passengerEmail)
                .OrderByDescending(p => p.TimeAccepted)
                .FirstOrDefault()?
                .TimeFinished;

            if(timeFinished == null)
            {
                return new CooldownDto
                {
                    IsInCooldown = false,
                    Cooldown = TimeSpan.FromSeconds(0),
                };
            }

            //Calculate time passed since last finished ride
#if DEBUG
            var timePassed = DateTime.Now.ToUniversalTime() - timeFinished.Value.ToUniversalTime();
#else
            var timePassed = DateTime.Now.ToUniversalTime() - timeFinished.Value;
#endif

            int coolDownTime = Convert.ToInt32(Environment.GetEnvironmentVariable("CoolDownTime"));
            var minutesLeft = TimeSpan.FromMinutes(coolDownTime) - timePassed;

            //If cooldown is completed, then set to 0 to avoid negative values
            if (minutesLeft.TotalSeconds <= 0)
            {
                minutesLeft = TimeSpan.FromSeconds(0);
            }

            return new CooldownDto
            {
                IsInCooldown = minutesLeft.TotalSeconds > 0,
                Cooldown = minutesLeft,
            };
        }
    }
}
