using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IRideService rideService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public PassengerService(IRideService rideService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.rideService = rideService;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Passenger> GetById(int id)
        {
            return await unitOfWork.PassengerRepository.GetById(id);
        }
        public async Task<IEnumerable<PassengerDto>> GetPassengersByRide(int rideId)
        {
            var ride = await rideService.GetById(rideId);
            if (ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passengers = ride.Passengers;
            var dtos = mapper.Map<IEnumerable<PassengerDto>>(passengers);
            return dtos;
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
            if(!canAccept)
            {
                throw new Exception("REQUEST_NOT_AVAILABLE_OR_ACCEPTED");
            }

            passenger.TimeAccepted = DateTime.Now;
            
            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);
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

            bool canIgnore = passenger.TimeAccepted == null && passenger.TimeIgnored == null && passenger.TimeCancelled == null;
            if (!canIgnore)
            {
                throw new Exception("REQUEST_NOT_AVAILABLE_OR_ACCEPTED");
            }

            passenger.TimeIgnored = DateTime.Now;

            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);
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

            bool canCancel = passenger.TimeCancelled != null;
            if (!canCancel)
            {
                throw new Exception("REQUEST_ALREADY_CANCELED");
            }

            passenger.TimeCancelled = DateTime.Now;

            unitOfWork.PassengerRepository.Update(passenger);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<PassengerDto>(passenger);
            return dto;
        }
    }
}