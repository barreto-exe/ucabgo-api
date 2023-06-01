using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class RideService : IRideService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public RideService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
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

            var ridesDtos = mapper.Map<List<RideDto>>(rides.ToList());
            return ridesDtos;
        }

        public async Task<IEnumerable<RideDto>> GetAll(string driverEmail, bool onlyAvailable = false)
        {
            var items = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.DestinationNavigation,
                    r => r.DriverNavigation,
                    r => r.Evaluations);

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
        public async Task<IEnumerable<PassengerDto>> GetPassengers(int rideId)
        {
            var ride = await GetById(rideId);
            if (ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passengers = ride.Passengers;
            var dtos = mapper.Map<IEnumerable<PassengerDto>>(passengers);
            return dtos;
        }
    }
}
