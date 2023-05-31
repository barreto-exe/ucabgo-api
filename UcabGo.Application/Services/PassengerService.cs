using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
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

            //Validate if initial location exists
            var initialLocation = await locationService.GetById(input.InitialLocation);
            if (initialLocation == null || initialLocation.User != idUser)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }

            //Validate if user is already in a ride
            var rideDto = await CurrentRide(input.Email);
            if (rideDto != null)
            {
                throw new Exception("ALREADY_IN_RIDE");
            }

            //Validate if available seats
            var activePassengers = ride.Passengers.Where(p => p.TimeAccepted != null && p.TimeCancelled == null && p.TimeIgnored == null);
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

        public async Task<RideDto?> CurrentRide(string passengerEmail)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(passengerEmail)).Id;

            //Get passengers
            var passengerList = unitOfWork.PassengerRepository.GetAll();

            var passenger = passengerList
                .FirstOrDefault(p =>
                    p.User == idUser &&
                    p.TimeAccepted != null &&
                    p.TimeCancelled == null &&
                    p.TimeIgnored == null);
            if (passenger == null)
            {
                return null;
            }

            //Get ride
            var ride = await rideService.GetById(passenger.Ride);
            if (ride == null)
            {
                return null;
            }

            return mapper.Map<RideDto>(ride);
        }
    }
}
